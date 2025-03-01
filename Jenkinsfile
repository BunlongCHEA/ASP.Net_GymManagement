pipeline{
    agent any
    parameters {
        string(name: 'BRANCH', defaultValue: 'main', description: 'Branch to build from')
        booleanParam(name: 'USE_HTTPS', defaultValue: false, description: 'Run on HTTPS (Requires Certificate)')
        string(name: 'CERTIFICATE_PATH', defaultValue: '', description: 'Pass Only Path to SSL Certificate (Required if HTTPS is enabled)')
        string(name: 'CERTIFICATE_NAME', defaultValue: '', description: 'SSL Certificate Name (Required if HTTPS is enabled)')
        string(name: 'CERTIFICATE_KEY_NAME', defaultValue: '', description: 'SSL Certificate Key Name (Required if HTTPS is enabled)')
    }
    environment {
        PROJECT_NAME = 'Gym_ManagementSystem'
        NGINX_CONTAINER = 'nginx_proxy'
        DOCKER_COMPOSE_FILE = 'docker-compose.yaml'
        SQL_PASSWORD = 't01UA<2%7~v45'
        DB_CONNECTION_STRING = 'Server=db;Database=GymManagementSystem;User Id=sa;Password=t01UA<2%7~v45;TrustServerCertificate=True;'
        DOTNET_ROOT = "/usr/lib/dotnet"
        PATH = "/usr/lib/dotnet:/usr/lib/dotnet/tools:/var/lib/jenkins/.dotnet/tools:$PATH"
    }
    stages{        
        stage('Modify Docker-Compose for Database Credential') {
            steps {
                script {
                    sh """
                    echo '***Before config ${DOCKER_COMPOSE_FILE}...'
                    cat ${DOCKER_COMPOSE_FILE}
                    sed -i 's|t01UA<2%7~v4|${SQL_PASSWORD}|g' ${DOCKER_COMPOSE_FILE}
                    sed -i 's|Server=db;Database=GymManagementSystem;User Id=sa;Password=t01UA<2%7~v4;TrustServerCertificate=True;|${DB_CONNECTION_STRING}|g' ${DOCKER_COMPOSE_FILE}
                    echo '***After config ${DOCKER_COMPOSE_FILE}...'
                    cat ${DOCKER_COMPOSE_FILE}
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

        stage("Install dotnet tool for migration") {
            steps {
                script {
                    sh """
                    export DOTNET_ROOT=/usr/lib/dotnet
                    export PATH=/usr/lib/dotnet:/usr/lib/dotnet/tools:/var/lib/jenkins/.dotnet/tools:\$PATH

                    dotnet --info
                    dotnet-ef --version
                    dotnet ef database update
                    """
                }
            }
        }
    }
    post{
        always{
            echo "========always========"
        }
        success{
            echo "========pipeline executed successfully ========"
        }
        failure{
            echo "========pipeline execution failed========"
        }
    }
}