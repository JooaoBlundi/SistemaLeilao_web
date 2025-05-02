// Placeholder for SignalR client logic

// Ensure the SignalR client library is loaded (e.g., via CDN or local file in _Layout.cshtml)
// <script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/8.0.0/signalr.min.js"></script>

$(document).ready(function () {
    console.log("SignalR script loaded. Attempting to connect...");

    // TODO: Get API base URL dynamically (e.g., from configuration)
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:44304/auctionHub")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    async function start() {
        try {
            await connection.start();
            console.log("SignalR Connected.");
            // Example: Join a specific auction group if on an auction page
            // const auctionId = $('#currentAuctionId').val(); // Assuming an input holds the ID
            // if (auctionId) {
            //     connection.invoke("JoinAuctionGroup", auctionId).catch(err => console.error(err.toString()));
            // }
        } catch (err) {
            console.error("SignalR Connection Failed: ", err);
            // Optionally retry connection after a delay
            setTimeout(start, 5000);
        }
    };

    connection.onclose(async () => {
        console.log("SignalR Connection Closed.");
        // Optionally attempt to reconnect
        await start();
    });

    // --- Define handlers for messages from the server --- 

    // Handler for receiving bid updates
    connection.on("ReceiveBidUpdate", (auctionId, user, bidAmount) => {
        console.log(`Received Bid Update for Auction ${auctionId}: User ${user}, Amount ${bidAmount}`);
        // TODO: Update the UI to reflect the new bid
        // Example: Find the auction element and update the latest bid display
        // Show notification (e.g., using a toast library or updating the bell icon)
        showNotification(`Novo lance de ${bidAmount} por ${user} no leilão ${auctionId}!`);
    });

    // Handler for receiving auction end notifications
    connection.on("ReceiveAuctionEnd", (auctionId, winner, finalPrice) => {
        console.log(`Received Auction End for Auction ${auctionId}: Winner ${winner}, Price ${finalPrice}`);
        // TODO: Update the UI to show the auction has ended and who won
        // Show notification
        showNotification(`Leilão ${auctionId} finalizado! Vencedor: ${winner}, Preço Final: ${finalPrice}`);
    });

    // --- Helper function for showing notifications (placeholder) ---
    function showNotification(message) {
        // TODO: Implement actual notification display logic
        // This could involve: 
        // 1. Using a JavaScript notification library (like Toastr)
        // 2. Updating a counter on the bell icon
        // 3. Appending messages to a notification dropdown
        console.log("NOTIFICATION: ", message);
        alert("Notificação: " + message); // Simple alert for now
        // Update bell icon (example)
        const bell = $('#notificationBell'); // Assuming an ID for the bell icon
        if (bell.length) {
            let count = parseInt(bell.attr('data-count') || '0') + 1;
            bell.attr('data-count', count);
            bell.addClass('has-notifications'); // Add a class for styling
            // Update a badge if present
            bell.find('.badge').text(count).show(); 
        }
    }


    // Start the connection
    start();

});

