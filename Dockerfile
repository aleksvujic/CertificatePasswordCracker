#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["CertificatePasswordCracker/CertificatePasswordCracker.csproj", "CertificatePasswordCracker/"]
RUN dotnet restore "CertificatePasswordCracker/CertificatePasswordCracker.csproj"
COPY . .
WORKDIR "/src/CertificatePasswordCracker"
RUN dotnet build "CertificatePasswordCracker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CertificatePasswordCracker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
LABEL rep=certificate-password-cracker
ENTRYPOINT ["dotnet", "CertificatePasswordCracker.dll"]
