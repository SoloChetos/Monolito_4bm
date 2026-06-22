pipeline {
    agent none
    stages {
        stage('Checkout') {
            agent any
            steps {
                checkout scm
                stash name: 'sources', includes: '**'
            }
        }
        stage('Restaurar y Compilar') {
            agent { label 'windows' }
            steps {
                unstash 'sources'
                bat '''
                    echo Restaurando paquetes NuGet con detalle...
                    nuget restore Monolito_4bm.sln -Verbosity detailed -Source https://api.nuget.org/v3/index.json -Force
                    
                    echo Mostrando paquetes restaurados (packages folder)...
                    if exist packages dir packages
                    
                    echo Compilando solucion...
                    msbuild Monolito_4bm.sln /p:Configuration=Release /p:Platform="Any CPU"
                '''
            }
        }
        stage('Publicar') {
            agent { label 'windows' }
            steps {
                bat '''
                    echo Publicando archivos...
                    if not exist "C:\\publicado\\Monolito" mkdir "C:\\publicado\\Monolito"
                    xcopy /E /Y "Monolito_4bm\\bin" "C:\\publicado\\Monolito"
                '''
                archiveArtifacts artifacts: 'C:\\publicado\\Monolito\\**', fingerprint: true
            }
        }
    }
    post {
        success {
            echo 'Pipeline completado con exito.'
        }
        failure {
            echo 'Pipeline fallo. Revisa los logs.'
        }
    }
}