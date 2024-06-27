////function requestPermission() {
////  console.log('Requesting permission...');
////  Notification.requestPermission().then((permission) => {
////    if (permission === 'granted') {
////      console.log('Notification permission granted.');
////    }
////}).catch((err=>console.log("Unable to get permission to notify. ", err)));
////}
messaging.onBackgroundMessage((payload) => {
    console.log('[firebase-messaging-sw.js] Received background message ', payload);
    // Customize notification here
    const notificationTitle = 'Background Message Title';
    const notificationOptions = {
        body: 'Background Message body.',
        icon: '/firebase-logo.png'
    };

    self.registration.showNotification(notificationTitle,
        notificationOptions);
});