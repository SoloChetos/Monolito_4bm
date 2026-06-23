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
        stage('Ejecutar Pruebas') {
            agent { label 'windows' }
            steps {
                unstash 'sources'
                bat '''
                    echo [PRUEBA] Generando reporte de pruebas de conexion a bases de datos...
                    powershell -ExecutionPolicy Bypass -File pruebas_conexion.ps1
                '''
                junit testResults: 'test-results.xml', skipPublishingChecks: true
            }
        }
        stage('Publicar') {
            agent { label 'windows' }
            steps {
                bat '''
                    echo Publicando archivos...
                    if not exist "publish_output" mkdir "publish_output"
                    xcopy /E /Y "Monolito_4bm\\*" "publish_output\\"
                '''
                archiveArtifacts artifacts: 'publish_output/**', fingerprint: true
            }
        }
        stage('Desplegar en IIS') {
            agent { label 'windows' }
            steps {
                bat '''
                    echo Deteniendo Application Pool...
                    %windir%\\system32\\inetsrv\\appcmd stop apppool /apppool.name:"Monolito4BM"
                    echo Copiando archivos a IIS...
                    if not exist "C:\\inetpub\\wwwroot\\Monolito" mkdir "C:\\inetpub\\wwwroot\\Monolito"
                    xcopy /E /Y "publish_output" "C:\\inetpub\\wwwroot\\Monolito"
                    echo Iniciando Application Pool...
                    %windir%\\system32\\inetsrv\\appcmd start apppool /apppool.name:"Monolito4BM"
                '''
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