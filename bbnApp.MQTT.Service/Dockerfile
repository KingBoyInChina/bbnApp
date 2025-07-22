# 使用官方.NET 8 SDK镜像作为构建环境
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 复制解决方案文件（从bbnApp目录）
COPY bbnApp/bbnApp.sln .

# 复制各项目文件（从同级目录）
COPY bbnApp.MQTT.Service/bbnApp.MQTT.Service.csproj ./bbnApp.MQTT.Service/
COPY bbnApp.GrpcClients/bbnApp.GrpcClients.csproj ./bbnApp.GrpcClients/
COPY bbnApp.Mapper/bbnApp.Mapper.csproj ./bbnApp.Mapper/
COPY bbnApp.Common/bbnApp.Common.csproj ./bbnApp.Common/
COPY bbnApp.DTOs/bbnApp.DTOs.csproj ./bbnApp.DTOs/
COPY bbnApp.Protos/bbnApp.Protos.csproj ./bbnApp.Protos/
COPY bbnApp.Infrastructure/bbnApp.Infrastructure.csproj ./bbnApp.Infrastructure/
COPY bbnApp.Domain/bbnApp.Domain.csproj ./bbnApp.Domain/
COPY bbnApp.Core/bbnApp.Core.csproj ./bbnApp.Core/
COPY bbnApp.Share/bbnApp.Share.csproj ./bbnApp.Share/

# 恢复NuGet包
RUN dotnet restore "bbnApp.MQTT.Service/bbnApp.MQTT.Service.csproj"

# 复制所有源代码（从同级目录）
COPY bbnApp.MQTT.Service/ ./bbnApp.MQTT.Service/
COPY bbnApp.GrpcClients/ ./bbnApp.GrpcClients/
COPY bbnApp.Mapper/ ./bbnApp.Mapper/
COPY bbnApp.Common/ ./bbnApp.Common/
COPY bbnApp.DTOs/ ./bbnApp.DTOs/
COPY bbnApp.Protos/ ./bbnApp.Protos/
COPY bbnApp.Infrastructure/ ./bbnApp.Infrastructure/
COPY bbnApp.Domain/ ./bbnApp.Domain/
COPY bbnApp.Core/ ./bbnApp.Core/
COPY bbnApp.Share/ ./bbnApp.Share/

# 构建项目
WORKDIR "/src/bbnApp.MQTT.Service"
RUN dotnet build "bbnApp.MQTT.Service.csproj" -c Release -o /app/build

# 发布项目
FROM build AS publish
RUN dotnet publish "bbnApp.MQTT.Service.csproj" -c Release -o /app/publish \
    --runtime linux-x64 \
    --self-contained false \
    -p:PublishReadyToRun=true

# 使用.NET 8运行时镜像
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 1883
COPY --from=publish /app/publish .

# 设置入口点
ENTRYPOINT ["dotnet", "bbnApp.MQTT.Service.dll"]
