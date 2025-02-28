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
        NGINX_DORMAIN = 'yourdormain.com'
        DOCKER_COMPOSE_FILE = 'docker-compose.yml'
        SQL_PASSWORD = 't01UA<2%7~v4'
        DB_CONNECTION_STRING = 'Server=db;Database=GymManagementSystem;User Id=sa;Password=t01UA<2%7~v4;TrustServerCertificate=True;'
    }
    stages{        
        stage('Modify Docker-Compose for Database Credential') {
            steps {
                script {
                    sh """
                    sed -i 's|t01UA<2%7~v4|${SQL_PASSWORD}|g' ${DOCKER_COMPOSE_FILE}
                    sed -i 's|Server=db;Database=GymManagementSystem;User Id=sa;Password=t01UA<2%7~v4;TrustServerCertificate=True;|${DB_CONNECTION_STRING}|g' ${DOCKER_COMPOSE_FILE}
                    """
                }
            }
        }

        stage('Handle HTTPS Configuration') {
            when {
                expression { params.USE_HTTPS }
            }
            steps {
                script {
                    if (!params.CERTIFICATE_PATH) {
                        error "HTTPS is enabled but no certificate paths were provided!"
                    }

                    sh """
                    echo 'Configuring docker-compose.yaml for located certs path for HTTPS...'
                    sed -i 's|./certs|${params.CERTIFICATE_PATH}/g' ${DOCKER_COMPOSE_FILE}
                    echo 'Configuring Nginx for HTTPS...'
                    sed -i 's|yourdomain.com|${NGINX_DORMAIN}|g' ${NGINX_CONTAINER}
                    sed -i 's|server.crt|${params.CERTIFICATE_NAME}/g' ${NGINX_CONTAINER}
                    sed -i 's|server.key|${params.CERTIFICATE_KEY_NAME}/g' ${NGINX_CONTAINER}
                    """
                }
            }
        }

        stage("Build and Start Containers") {
            steps {
                script {
                    sh 'docker-compose up -d'

                    sh """
                    echo 'Waiting for SQL Server to be ready...'
                    until nc -z sqlserver 1433; do
                        sleep 2
                    done
                    echo "SQL Server is ready!"
                    """
                }
            }
        }

        stage("Apply Database Migrations") {
            steps {
                script {
                    sh 'dotnet ef database update'
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