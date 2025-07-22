# 使用官方.NET 8 SDK镜像作为构建环境
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 复制解决方案文件（从bbnApp目录）
COPY bbnApp/bbnApp.sln .

# 复制各项目文件（从同级目录）
COPY bbnApp.MQTT.DataReciveClient/bbnApp.MQTT.DataReciveClient.csproj ./bbnApp.MQTT.DataReciveClient/
COPY bbnApp.InfluxDb.Client/bbnApp.InfluxDb.Client.csproj ./bbnApp.InfluxDb.Client/
COPY bbnApp.GrpcClients/bbnApp.GrpcClients.csproj ./bbnApp.GrpcClients/
COPY bbnApp.Mapper/bbnApp.Mapper.csproj ./bbnApp.Mapper/
COPY bbnApp.DTOs/bbnApp.DTOs.csproj ./bbnApp.DTOs/
COPY bbnApp.Protos/bbnApp.Protos.csproj ./bbnApp.Protos/
COPY bbnApp.DTOs/ ./bbnApp.DTOs/
COPY bbnApp.Protos/ ./bbnApp.Protos/
COPY bbnApp.Infrastructure/bbnApp.Infrastructure.csproj ./bbnApp.Infrastructure/
COPY bbnApp.Domain/bbnApp.Domain.csproj ./bbnApp.Domain/
COPY bbnApp.Core/bbnApp.Core.csproj ./bbnApp.Core/
COPY bbnApp.RabbitMQ/bbnApp.RabbitMQ.csproj ./bbnApp.RabbitMQ/
COPY bbnApp.Share/bbnApp.Share.csproj ./bbnApp.Share/
COPY bbnApp.Common/bbnApp.Common.csproj ./bbnApp.Common/


# 恢复NuGet包
RUN dotnet restore "bbnApp.MQTT.DataReciveClient/bbnApp.MQTT.DataReciveClient.csproj"

# 复制所有源代码（从同级目录）
COPY bbnApp.MQTT.DataReciveClient/ ./bbnApp.MQTT.DataReciveClient/
COPY bbnApp.InfluxDb.Client/ ./bbnApp.InfluxDb.Client/
COPY bbnApp.GrpcClients/ ./bbnApp.GrpcClients/
COPY bbnApp.Mapper/ ./bbnApp.Mapper/
COPY bbnApp.DTOs/ ./bbnApp.DTOs/
COPY bbnApp.Protos/ ./bbnApp.Protos/
COPY bbnApp.Infrastructure/ ./bbnApp.Infrastructure/
COPY bbnApp.Domain/ ./bbnApp.Domain/
COPY bbnApp.Core/ ./bbnApp.Core/
COPY bbnApp.RabbitMQ/ ./bbnApp.RabbitMQ/
COPY bbnApp.Common/ ./bbnApp.Common/
COPY bbnApp.Share/ ./bbnApp.Share/

# 构建项目
WORKDIR "/src/bbnApp.MQTT.DataReciveClient"
RUN dotnet build "bbnApp.MQTT.DataReciveClient.csproj" -c Release -o /app/build

# 发布项目
FROM build AS publish
RUN dotnet publish "bbnApp.MQTT.DataReciveClient.csproj" -c Release -o /app/publish \
    --runtime linux-x64 \
    --self-contained false \
    -p:PublishReadyToRun=true

# 使用.NET 8运行时镜像
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8801
COPY --from=publish /app/publish .

# 设置入口点
ENTRYPOINT ["dotnet", "bbnApp.MQTT.DataReciveClient.dll"]
