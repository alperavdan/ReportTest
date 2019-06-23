FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
RUN apt-get update && \   
    apt-get install -y gnupg  libgconf-2-4 wget && \
    wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add - && \
    sh -c 'echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google.list' && \
    apt-get update && \
    apt-get install -y google-chrome-unstable --no-install-recommends

ENV chrome:launchOptions:args --no-sandbox

WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["JSReportTestApp/JSReportTestApp.csproj", "JSReportTestApp/"]
RUN dotnet restore "JSReportTestApp/JSReportTestApp.csproj"
COPY . .
WORKDIR "/src/JSReportTestApp"
RUN dotnet build "JSReportTestApp.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "JSReportTestApp.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "JSReportTestApp.dll"]