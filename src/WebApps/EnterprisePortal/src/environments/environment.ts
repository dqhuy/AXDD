export const environment = {
  production: false,
  apiGatewayUrl: 'http://localhost:5000',
  apiEndpoints: {
    auth: 'http://localhost:5001',
    masterData: 'http://localhost:5002',
    enterprise: 'http://localhost:5003',
    investment: 'http://localhost:5004',
    fileManager: 'http://localhost:5005',
    report: 'http://localhost:5006',
    notification: 'http://localhost:5007',
    logging: 'http://localhost:5008',
    search: 'http://localhost:5009',
    gis: 'http://localhost:5010'
  },
  signalR: {
    notificationHubUrl: 'http://localhost:5007/hubs/notifications'
  }
};
