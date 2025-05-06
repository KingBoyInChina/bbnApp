@echo off
REM ����������
if "%1"=="" (
    echo �������ṩ���ݿ����������ơ�
    pause
    exit /b
)

set DB_CONTEXT=%1

REM ��ȡǨ�������ļ�
if not exist migrations.txt (
    echo ����δ�ҵ� migrations.txt �ļ���
    pause
    exit /b
)

REM ���Ǩ��
for /f %%i in (migrations.txt) do (
    echo �������Ǩ�ƣ�%%i
    dotnet ef migrations add %%i --context %DB_CONTEXT% --startup-project ../bbnApp.Service

    if %errorlevel% neq 0 (
        echo Ǩ�����ʧ�ܣ����������Ϣ��
        pause
        exit /b
    )
)

REM �������ݿ�
echo ���ڸ������ݿ�...
dotnet ef database update --context %DB_CONTEXT% --startup-project ../bbnApp.Service

if %errorlevel% neq 0 (
    echo ���ݿ����ʧ�ܣ����������Ϣ��
    pause
    exit /b
)

echo Ǩ�ƺ����ݿ���³ɹ���ɣ�
pause
