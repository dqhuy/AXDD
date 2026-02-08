export const environment = {
  production: true,
  apiGatewayUrl: '/api',
  apiEndpoints: {
    auth: '/api/auth',
    masterData: '/api/masterdata',
    enterprise: '/api/enterprise',
    investment: '/api/investment',
    fileManager: '/api/filemanager',
    report: '/api/report',
    notification: '/api/notification',
    logging: '/api/logging',
    search: '/api/search',
    gis: '/api/gis'
  },
  signalR: {
    notificationHubUrl: '/api/notification/hubs/notifications'
  }
};
