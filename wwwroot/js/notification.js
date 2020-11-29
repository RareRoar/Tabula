"use strict";

var notificationConnection;
openConnection();
function openConnection() {
    $("#msg").html("Preparing...");
    notificationConnection = new signalR.HubConnectionBuilder().withUrl("/notification").build();
    notificationConnection
        .start()
        .then(() => {
            $("#msg").html("Connection established: " + notificationConnection.connection.connectionId);
        })
        .catch(() => {
            alert("Error while establishing connection");
        });
}
notificationConnection.on("DisplayNotification", (message) => {
    var li = document.createElement("li");
    li.textContent = message;
    document.getElementById("notificationContent").appendChild(li);
    window.setTimeout(function () {
        $("#notificationTitle").html("Winds of change are blowing...");
    },
        10000);
});