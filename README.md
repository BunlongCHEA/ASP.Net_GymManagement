# Requirement Before Clone And Run

NOTE: We run using user - jenkins - so ensure user add to group Docker, and Root to avoid permmission

    # Login as root first, then add the Jenkins user to the docker group
    sudo usermod -aG docker jenkins
    sudo usermod -aG root jenkins
    
    # Restart Jenkins to apply the changes
    sudo systemctl restart Jenkins
    
    # Ensure docker is listed in the output.
    groups Jenkins
    
    # Go to jenkins user, and run docker to see if access
    docker ps

Here is how to create SSH Key

    # Go to user Jenkins
    su - jenkins
    
    # Check if Exist key
    ls -la ~/.ssh/
    
    # Generate new SSH key
    ssh-keygen -t ed25519 -C "<<Your GitHub Gmail>>"
    ssh-keygen -t rsa -b 4096 -C "<<Your GitHub Gmail>>"
    
    # Start the SSH agent:
    eval "$(ssh-agent -s)"
    
    # Add your key to agent Or use ~/.ssh/id_rsa if that's your key
    ssh-add ~/.ssh/id_ed25519
    #OR
    ssh-add ~/.ssh/id_rsa
    
    # Verify the Key Is Added
    ssh-add -l
    
    # Copy the entire key and add it to GitHub → Settings → SSH Keys.
    cat ~/.ssh/id_ed25519.pub

In case, cannot clone this GitHub on application Jenkins, ensure you create SSH key and pass Public-Key to GitHub, then create and config **.ssh/config**

    nano /var/lib/jenkins/.ssh/config

    # Add and change User to your username, while find your id_rsa file SSH Private-Key
    # You can add to path **~/.ssh** for root user to avoid not able to clone GitHub
    # Host github.com: Specifies the rule applies to GitHub connections.
    # User git: The username used to connect to GitHub.
    # IdentityFile ~/.ssh/id_rsa: Ensures the correct SSH key is used.
    # StrictHostKeyChecking no: Prevents Git from asking for confirmation when connecting for the first time.
    Host github.com
        User BunlongCHEA
        IdentityFile ~/.ssh/id_rsa
        StrictHostKeyChecking no

Test Again, you should see - Hi YourGitHubUserName! You've successfully authenticated, but GitHub does not provide shell access.
    
    ssh -T git@github.com

If Still Error -- Failed to connect to repository: Command "git ls-remote -h - git@github.com:your-repository/your-project.git HEAD" returned status code 128 --
This issue occurs because the GitHub server is not recognized by your machine’s known_hosts file, which prevents Jenkins from validating the connection due to strict host key checking.
    
    ssh-keyscan github.com >> ~/.ssh/known_hosts

Restart Jenkins
    
    sudo systemctl restart jenkins

# Jenkinsfile

After clone this project, need to make some change to **environment**: 

*APP_URL : To change accord to your OS, and IP run, since run on HTTP

*SQL_PASSWORD : Can change any password. For more info and rule for password and parameter, see https://hub.docker.com/r/microsoft/mssql-server

*DB_CONNECTION_STRING : Database server must be the same to SQL_PASSWORD, since need to connected to database, while other config can change based on your need

*TELEGRAM_BOT_TOKEN : Find @BotFather on Telegram, and register API-Token

*TELEGRAM_CHAT_ID : Find @getidsbot on Telegram, after create your group on Telegram and add - @BotFather & @getidsbot - to group to get **Chat_ID** (You can Remove @getidsbot later after receive Chat_ID)

    pipeline{
      agent any
      parameters {
          string(name: 'BRANCH', defaultValue: 'main', description: 'Branch to build from')
          choice(name: 'APP_ENV', choices: ['Production', 'Development', 'Staging'], description: 'Select the deployment environment')
          booleanParam(name: 'USE_HTTPS', defaultValue: false, description: 'Run on HTTPS (Requires Certificate)')
          string(name: 'CERTIFICATE_PATH', defaultValue: '', description: 'Pass Only Path to SSL Certificate (Required if HTTPS is enabled)')
          string(name: 'CERTIFICATE_NAME', defaultValue: '', description: 'SSL Certificate Name (Required if HTTPS is enabled)')
          string(name: 'CERTIFICATE_KEY_NAME', defaultValue: '', description: 'SSL Certificate Key Name (Required if HTTPS is enabled)')
      }
      environment {
          PROJECT_NAME = 'Gym_ManagementSystem'
          APP_URL= 'http://<<Your OS IP>>/'
          NGINX_CONTAINER = 'nginx_proxy'
          DOCKER_COMPOSE_FILE = 'docker-compose.yaml'
          APPSETTING_FILE = './Gym_ManagementSystem/appsettings.json'
          SQL_PASSWORD = 't01UA<2%7~v45'
          DB_CONNECTION_STRING = 'Server=<<Your OS IP>>,1433;Database=GymManagementSystem;User Id=sa;Password=t01UA<2%7~v45;TrustServerCertificate=True;'
          DOTNET_ROOT = "/usr/lib/dotnet"
          PATH = "/usr/lib/dotnet:/usr/lib/dotnet/tools:/var/lib/jenkins/.dotnet/tools:$PATH"
          PROJECT_PATH = '/var/lib/jenkins/workspace/ASP DotNet Core/Gym/Gym_ManagementSystem'
  
          TELEGRAM_BOT_TOKEN = '<<Telegram HTTP API Token>>'
          TELEGRAM_CHAT_ID = 'Chat_ID'
      }
      stages{        
          stage('Modify Docker-Compose for Database Credential') {
              steps {
                  script {
                      sh """
                      echo '***Before config ${DOCKER_COMPOSE_FILE}...'
                      cat ${DOCKER_COMPOSE_FILE}
  
                      sed -i 's|t01UA<2%7~v4|${SQL_PASSWORD}|g' ${DOCKER_COMPOSE_FILE}
                      sed -i 's|Server=sqlserver22;Database=GymManagementSystem;User Id=sa;Password=t01UA<2%7~v4;TrustServerCertificate=True;|${DB_CONNECTION_STRING}|g' ${DOCKER_COMPOSE_FILE}
  
                      pwd
  
                      sed -i 's|Server=localhost;Database=GymManagementSystem;User Id=sa;Password=12345;TrustServerCertificate=True;Integrated security=False;MultipleActiveResultSets=true;Encrypt=False;|${DB_CONNECTION_STRING}|g' "${APPSETTING_FILE}"
  
                      echo '***After config ${DOCKER_COMPOSE_FILE}...'
                      cat ${DOCKER_COMPOSE_FILE}
  
                      echo '***Check config ${APPSETTING_FILE}...'
                      cat ${APPSETTING_FILE}
                      """
                  }
              }
          }
  
          stage("Build and Start Containers") {
              steps {
                  script {
                      sh 'docker-compose -f ${DOCKER_COMPOSE_FILE} up -d'
                  }
              }
          }
  
          stage("Check dotnet tool") {
              steps {
                  script {
                      sh """
                      dotnet --info
                      dotnet-ef --version
                      pwd
                      dotnet ef database update --project '${PROJECT_PATH}'
                      """
                  }
              }
          }
      }
      post{
          always{
             script {
                  sh """
                      curl -s -X POST https://api.telegram.org/bot${TELEGRAM_BOT_TOKEN}/sendMessage \
                          -d chat_id=${TELEGRAM_CHAT_ID} \
                          -d parse_mode="HTML" \
                          -d disable_web_page_preview=true \
                          -d text="
                          🔔 <b>*Jenkins Build Notification*</b> 🔔
                          %0A📚<b>Stage</b>: Deploy ${PROJECT_NAME} \
                          %0A🟢<b>Status:</b> ${currentBuild.result} \
                          %0A🔢<b>Version:</b> ${params.APP_ENV}-${BUILD_NUMBER} \
                          %0A📌<b>Environment:</b> ${params.APP_ENV} \
                          %0A🔗<b>Application URL:</b> ${APP_URL} \
                          %0A👤<b>User Build:</b> ${BUILD_USER}"
                  """
             }
          }
      }
    }

# Tool to Run Program

Recommend to use IDE Microsoft Visual Studio Installer as this code originally run on this.

![Visual-Studio-installer.png : IDE tool](https://github.com/BunlongCHEA/ASP.Net_GymManagement/blob/main/Visual-Studio-installer.png)
