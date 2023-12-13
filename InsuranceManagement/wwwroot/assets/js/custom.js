$(document).ready(function () {
    var baseloc = $("#locat").val();

    // 3000 is 3 seconds
    $.sessionTimeout({
        title: 'Expiring: Your Current Session',
        keepAliveButton: 'Continue',
        keepAliveUrl: baseloc + '/Home/KeepAlive',
        logoutUrl: baseloc + 'Home/LogOff',
        redirUrl: baseloc + '/Auth/Login', 
        warnAfter: 180000,
        redirAfter: 300000,
        countdownMessage: 'Redirecting in {timer} seconds.'
    });
});
