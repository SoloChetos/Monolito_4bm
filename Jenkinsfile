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
                    echo Restaurando paquetes NuGet...
                    nuget restore Monolito_4bm.sln
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