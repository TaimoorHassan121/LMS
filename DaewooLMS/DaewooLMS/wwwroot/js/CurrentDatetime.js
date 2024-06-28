window.onload = function () {
    // Get the current date and time
    const now = new Date();

    // Get the timezone offset in minutes and convert to milliseconds
    const timezoneOffset = now.getTimezoneOffset() * 60000;

    // Adjust the time to the local time zone
    const localDate = new Date(now.getTime() - timezoneOffset);

    // Format the date and time to match the input type 'datetime-local'
    const formattedDate = localDate.toISOString().slice(0, 16);

    // Set the value of the input field
    document.getElementById('qdDate').value = formattedDate;
}