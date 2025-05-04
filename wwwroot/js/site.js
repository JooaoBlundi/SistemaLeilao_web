// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


// Optional: Clear notifications logic
    $(\'#clearNotificationsBtn\').on(\'click\', function() {ationList').empty().append('<li class="list-group-item text-muted text-center no-notifications">Nenhuma notificação nova.</li>');
        // Also reset the counter on the bell icon
        $('.notification-count').text('0').hide();
        $('#notificationBell').data('count', 0);
    });
});

