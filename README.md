# Laboratoire 🧪🔬

[![Language Stats](https://github-readme-stats.vercel.app/api/top-langs/?username=theDevOli\&repo=laboratoire\&layout=compact\&theme=radical\&hide_border=true\&bg_color=00000000\&title_color=58a6ff\&text_color=8b949e\&hide=html,css,scss)](https://github.com/theDevOli/laboratoire)

Laboratoire is a full-stack web application designed for managing laboratory operations, experiments, and research data.

## 📊 Technology Stack

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Angular](https://img.shields.io/badge/Angular-DD0031?style=for-the-badge&logo=angular&logoColor=white)
![TypeScript](https://img.shields.io/badge/TypeScript-007ACC?style=for-the-badge&logo=typescript&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-4169E1?style=for-the-badge&logo=postgresql&logoColor=white)
![Node.js](https://img.shields.io/badge/Node.js-339933?style=for-the-badge&logo=nodedotjs&logoColor=white)

## 🚀 Features

* **Frontend**: Angular-based user interface with responsive design
* **Backend**: .NET 8.0 API with Entity Framework Core
* **Database**: SQL Server with advanced data modeling
* **Development**: Hot reload support for both frontend and backend
* **Environment Configuration**: Multi-environment support (Development/Production)

## 📁 Project Structure

```
laboratoire/
├── backend/
│   └── src/
│       └── external/
│           └── Laboratoire.UI/ # .NET 8.0 Backend API
├── frontend/
│   └── laboratoire/ # Angular Frontend Application
├── .gitignore # Comprehensive ignore rules
└── README.md # Project documentation
```

## 🛠️ Prerequisites

* **.NET 8.0 SDK**
* **Node.js** (v18 or higher)
* **Angular CLI** (v18 or higher)
* **SQL Server** (or SQL Server Express)
* **Git**

## ⚡ Quick Start

### Option 1: Run Both Services Simultaneously (Recommended)

```bash
# Using concurrently
yarn install 
yarn dev
```

### Option 2: Run Services Separately

**Backend (API):**

```bash
cd backend/src/external/Laboratoire.UI
DOTNET_ENVIRONMENT=Development dotnet run #Dev Mode
DOTNET_ENVIRONMENT=Production dotnet run #Prod Mode
```

API will be available at: `http://localhost:5000`

**Frontend (Angular):**

```bash
cd frontend/laboratoire
ng serve
```

Frontend will be available at: `http://localhost:4200`

## 🔧 Development

### Environment Configuration

The application uses environment variables for configuration:

```bash
export DOTNET_ENVIRONMENT=Development
export ASPNETCORE_ENVIRONMENT=Development
```

### Database Setup

Ensure PostgresSQL is running.

Update connection string in `appsettings.Development.json`.

### Building for Production

```bash
# Build backend
cd backend/src/external/Laboratoire.UI
dotnet publish Laboratoire.UI.csproj \
  -c Release \
  -r linux-x64 \
  --self-contained true \
  -p:PublishTrimmed=false \
  -o ./publish

# Build frontend
cd frontend/laboratoire
ng build --configuration production
```

## 📋 Available Scripts

| Command          | Description                      |
| ---------------- | -------------------------------- |
| ./start-dev.sh   | Starts both frontend and backend |
| make dev         | Starts using Makefile            |
| npm run dev\:all | Starts using npm scripts         |

## 🌐 API Documentation

The API provides RESTful endpoints for:

* **Laboratory Management**: CRUD operations for lab resources
* **Experiment Tracking**: Experiment lifecycle management
* **User Authentication**: JWT-based authentication system
* **Data Analysis**: Reporting and analytics endpoints

## 🐛 Troubleshooting

### Common Issues

* **Port Conflicts**: Check if ports 4200 (frontend) and 5000 (backend) are available
* **Database Connection**: Verify PostgreSQL is running and connection string is correct
* **Dependencies**: Run `npm install` in frontend directory if modules are missing

### Version Compatibility

* Angular CLI: 18.x
* .NET SDK: 8.0.x
* Node.js: 18.x or higher

## 🤝 Contributing

We welcome contributions! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🆘 Support

If you encounter any issues:

* Check the Issues page for existing solutions
* Create a new issue with detailed description
* Contact the development team

Happy Coding! 🎉

<div align="center">
[![Stars](https://img.shields.io/github/stars/theDevOli/laboratoire?style=social)](https://github.com/theDevOli/laboratoire/stargazers)
[![Forks](https://img.shields.io/github/forks/theDevOli/laboratoire?style=social)](https://github.com/theDevOli/laboratoire/network/members)
[![Issues](https://img.shields.io/github/issues/theDevOli/laboratoire)](https://github.com/theDevOli/laboratoire/issues)
[![License](https://img.shields.io/github/license/theDevOli/laboratoire)](https://github.com/theDevOli/laboratoire/blob/main/LICENSE)
</div>
