#!/bin/bash

# 函数：验证IP地址格式
validate_ip() {
    if [[ $1 =~ ^[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+$ ]]; then
        return 0
    else
        echo "无效的IP地址：$1"
        return 1
    fi
}

# 函数：验证MAC地址格式
validate_mac() {
    if [[ $1 =~ ^([0-9A-Fa-f]{2}:){5}[0-9A-Fa-f]{2}$ ]]; then
        return 0
    else
        echo "无效的MAC地址：$1"
        return 1
    fi
}

# 询问用户是否开启Swagger文档，默认为不开启
read -p "是否开启Swagger文档 (yes/no) [默认: no]: " use_swagger
use_swagger=${use_swagger:-no}
if [ "$use_swagger" == "yes" ]; then
    swagger="-e \"IsUseSwagger=true\" "
else
    swagger="-e \"IsUseSwagger=false\" "
fi

# 询问用户是否接入reCAPTCHA v3认证，默认为不开启
read -p "是否接入reCAPTCHA v3认证 (yes/no) [默认: no]: " use_recaptcha
use_recaptcha=${use_recaptcha:-no}
if [ "$use_recaptcha" == "yes" ]; then
    read -p "请输入reCAPTCHA v3的Client密钥: " recaptcha_client
    read -p "请输入reCAPTCHA v3的Server密钥: " recaptcha_server
    recaptcha="-e \"reCAPTCHA__Client=$recaptcha_client\" -e \"reCAPTCHA__Server=$recaptcha_server\" "
else
    recaptcha=""
fi

# 是否需要密码保护，默认为开启
read -p "是否需要密码保护 (yes/no) [默认: yes]: " use_password
use_password=${use_password:-yes}
if [ "$use_password" == "yes" ]; then
    read -p "请输入密码: " password
    password="-e \"WakeUp__Password=$password\" "
else
    password=""
fi

# 程序运行端口，默认为9000
read -p "请输入程序运行端口 [默认: 9000]: " port
port=${port:-9000}

# 询问用户是否需要开启端口映射，默认为不开启
read -p "是否需要开启端口映射 (yes/no) [默认: no](只有在Windows下使用WSL2运行Docker时需要开启): " use_port_mapping
use_port_mapping=${use_port_mapping:-no}
if [ "$use_port_mapping" == "yes" ]; then
    read -p "请输入端口映射的外部端口: " port_mapping
    port_mapping="-p \"$port_mapping:$port\" "
else
    port_mapping="--network host "
fi

port="-e \"Kestrel__Endpoints__Http__Url=http://*:$port\" "

# 询问需要开启的设备数量
read -p "请输入需要开启的设备数量: " device_count
devices=""
for (( i=0; i<$device_count; i++ ))
do
    read -p "请输入设备$i 的名称: " device_name
    
    while true; do
        read -p "请输入设备$i 的IP地址: " device_ip
        if [[ -z "$device_ip" ]] || validate_ip "$device_ip"; then
            break
        fi
    done
    
    while true; do
        read -p "请输入设备$i 的MAC地址: " device_mac
        if [[ -z "$device_mac" ]] || validate_mac "$device_mac"; then
            break
        fi
    done

    devices+="-e \"WakeUp__MacList__$i__Name=$device_name\" -e \"WakeUp__MacList__$i__IP=$device_ip\" -e \"WakeUp__MacList__$i__MAC=$device_mac\" "
done

# 询问子网掩码地址，默认为255.255.255.255
read -p "请输入子网掩码地址 [默认: 255.255.255.255](只有在旁路由环境下子网掩码可以使用255.255.255.255，其余情况要根据实际的ip进行计算，例如 ip为192.168.1.x 段，则子网掩码填写192.168.1.255): " subnet_address
subnet_address=${subnet_address:-255.255.255.255}
subnet="-e \"WakeUp__SubnetBroadcastAddress=$subnet_address\" "

# 构建完整的Docker命令
docker_command="docker run -d --restart=always $port_mapping$swagger$recaptcha$password$port$devices$subnet --name=remote_wakeup ghcr.io/cr-zhichen/remotewakeup:main"

# 执行Docker命令
echo "执行的Docker命令: $docker_command"
eval $docker_command