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
                    echo [PASO 1] Limpiando cache de paquetes NuGet...
                    if exist packages rmdir /S /Q packages
                    if exist "%USERPROFILE%\\.nuget\\packages" rmdir /S /Q "%USERPROFILE%\\.nuget\\packages"

                    echo [PASO 2] Restaurando paquetes NuGet desde cero...
                    nuget restore Monolito_4bm.sln -Source https://api.nuget.org/v3/index.json

                    echo [PASO 3] Verificando QRCoder.dll...
                    if not exist "packages\\QRCoder.1.8.0\\lib\\net40\\QRCoder.dll" (
                        echo *** QRCoder.dll no encontrado. Instalando manualmente...
                        nuget install QRCoder -Version 1.8.0 -OutputDirectory packages -Source https://api.nuget.org/v3/index.json
                    ) else (
                        echo *** QRCoder.dll encontrado correctamente.
                    )

                    echo [PASO 4] Compilando solucion...
                    msbuild Monolito_4bm.sln /p:Configuration=Release /p:Platform="Any CPU"
                '''
            }
        }
     stage('Publicar') {
    agent { label 'windows' }
    steps {
        bat '''
            echo Publicando archivos...
            if not exist "publish_output" mkdir "publish_output"
            xcopy /E /Y "Monolito_4bm\\bin" "publish_output"
        '''
        archiveArtifacts artifacts: 'publish_output/**', fingerprint: true
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