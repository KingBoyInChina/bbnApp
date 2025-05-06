@echo off
REM 检查参数数量
if "%1"=="" (
    echo 错误：请提供数据库上下文名称。
    pause
    exit /b
)

set DB_CONTEXT=%1

REM 读取迁移名称文件
if not exist migrations.txt (
    echo 错误：未找到 migrations.txt 文件。
    pause
    exit /b
)

REM 添加迁移
for /f %%i in (migrations.txt) do (
    echo 正在添加迁移：%%i
    dotnet ef migrations add %%i --context %DB_CONTEXT% --startup-project ../bbnApp.Service

    if %errorlevel% neq 0 (
        echo 迁移添加失败，请检查错误信息。
        pause
        exit /b
    )
)

REM 更新数据库
echo 正在更新数据库...
dotnet ef database update --context %DB_CONTEXT% --startup-project ../bbnApp.Service

if %errorlevel% neq 0 (
    echo 数据库更新失败，请检查错误信息。
    pause
    exit /b
)

echo 迁移和数据库更新成功完成！
pause
