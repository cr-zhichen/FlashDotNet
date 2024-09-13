@echo off
:: 设置控制台为 UTF-8 编码，防止中文乱码
chcp 65001

REM 调用 PowerShell 脚本 replace.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File "replace.ps1"