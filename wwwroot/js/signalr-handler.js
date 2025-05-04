// Ensure the SignalR client library is loaded (e.g., via CDN or local file in _Layout.cshtml)
// <script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/8.0.0/signalr.min.js"></script>

// Ensure apiBaseUrl is defined (should be set in _Layout.cshtml)
if (typeof apiBaseUrl === 'undefined') {
    console.error("apiBaseUrl is not defined. SignalR cannot connect.");
    // Handle the error appropriately, maybe display a message to the user
}

$(document).ready(function () {
    console.log("SignalR script loaded. Attempting to connect...");

    // Use the apiBaseUrl passed from _Layout.cshtml
    const hubUrl = apiBaseUrl.endsWith('/') ? `${apiBaseUrl}auctionHub` : `${apiBaseUrl}/auctionHub`;
    console.log(`Connecting to SignalR Hub at: ${hubUrl}`);

    const connection = new signalR.HubConnectionBuilder()
        .withUrl(hubUrl, {
            // Optional: Add access token if authentication is needed for the hub
            // accessTokenFactory: () => getAuthToken() // Implement getAuthToken() to retrieve the token (e.g., from cookie)
        })
        .configureLogging(signalR.LogLevel.Information)
        .build();

    // Function to get auth token from cookie (example)
    function getAuthToken() {
        const cookies = document.cookie.split(';');
        for (let i = 0; i < cookies.length; i++) {
            let cookie = cookies[i].trim();
            // Does this cookie string begin with the name we want?
            if (cookie.startsWith('AuthToken=')) {
                return cookie.substring('AuthToken='.length, cookie.length);
            }
        }
        return null; // Return null if token not found
    }

    async function start() {
        try {
            await connection.start();
            console.log("SignalR Connected.");
            // Example: Join a specific auction group if on an auction page
            // const auctionId = $("#currentAuctionId").val(); // Assuming an input holds the ID
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
        // Format currency
        const formattedAmount = parseFloat(bidAmount).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
        showNotification(`Novo lance de ${formattedAmount} por ${user} no leilão ID ${auctionId}!`);
        // TODO: Update specific UI elements related to the auction if needed
    });

    // Handler for receiving auction end notifications
    connection.on("ReceiveAuctionEnd", (auctionId, winner, finalPrice) => {
        console.log(`Received Auction End for Auction ${auctionId}: Winner ${winner}, Price ${finalPrice}`);
        const formattedPrice = parseFloat(finalPrice).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
        showNotification(`Leilão ID ${auctionId} finalizado! Vencedor: ${winner}, Preço Final: ${formattedPrice}`);
        // TODO: Update specific UI elements related to the auction if needed
    });

    // Handler for receiving new auction notifications (as requested by user)
    connection.on("ReceiveNewAuction", (auctionId, title, user) => {
        console.log(`Received New Auction Notification: ID ${auctionId}, Title: ${title}, User: ${user}`);
        showNotification(`Novo leilão "${title}" cadastrado por ${user} (ID: ${auctionId})!`);
        // TODO: Optionally refresh the auction list or add the new auction dynamically
    });

    // --- Helper function for showing notifications --- 
    function showNotification(message) {
        console.log("NOTIFICATION: ", message);

        const notificationList = $("#notificationList");
        const bell = $("#notificationBell");
        const badge = bell.find('.badge');
        const noNotificationsItem = notificationList.find('.no-notifications');

        // Remove 'no notifications' message if it exists
        if (noNotificationsItem.length) {
            noNotificationsItem.remove();
        }

        // Add new notification to the top of the list
        const timestamp = new Date().toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });
        notificationList.prepend(`<li class="list-group-item">${message} <small class="text-muted float-end">${timestamp}</small></li>`);

        // Update bell icon counter
        if (bell.length && badge.length) {
            let count = parseInt(badge.text() || '0') + 1;
            badge.text(count).show();
            // Optional: Add a class for animation/styling
            bell.addClass('has-notifications'); 
        }
    }

    // --- Event Listeners for Modal --- 

    // Open modal on bell click
    $('#notificationBell').on('click', function() {
        $('#notificationModal').modal('show');
        // Optional: Reset badge count when modal is opened
        // const badge = $(this).find('.badge');
        // badge.text('0').hide();
        // $(this).removeClass('has-notifications');
    });

    // Clear notifications button
    $('#clearNotificationsBtn').on('click', function() {
        const notificationList = $("#notificationList");
        const bell = $("#notificationBell");
        const badge = bell.find('.badge');

        notificationList.empty().append('<li class="list-group-item text-muted text-center no-notifications">Nenhuma notificação nova.</li>');
        badge.text('0').hide();
        bell.removeClass('has-notifications');
    });

    // Start the connection
    start();

});

