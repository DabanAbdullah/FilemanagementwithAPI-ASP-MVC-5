
var toastMixin = Swal.mixin({
    toast: true,
    icon: 'success',
    title: 'General Title',
    animation: false,
    position: 'top-right',
    showConfirmButton: false,
    timer: 2000,
    timerProgressBar: true,
    didOpen: (toast) => {
        toast.addEventListener('mouseenter', Swal.stopTimer)
        toast.addEventListener('mouseleave', Swal.resumeTimer)
    }
});



var actiontopost = "", authenticated = "false";
var allcookie = "", _saml_sp = "", _saml_idp = "", opensamlreq = "", opensamlreqname = "", opensamlreqvalue = "", _redirection_state = "", _redirect_user_idp = "", ShibSessionID = "", newlink = "", formid = "", cookietime = "", relaystate = "", cookietime = ""; SAMLResponse = "";
var authstate, allcookie;

var cookiereq = "";


function authenticate() {
   
    var host = 'http://' + window.location.host + '/';
   
    $.ajax({
        type: "GET",
        url: "/WTC/wtc1Async",
        data: '',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var cookie = response.cookie;
            var content = response.content;
            // make a new parser
            const parser = new DOMParser();

            // convert html string into DOM
            const document2 = parser.parseFromString(response.content, "text/html");
            var action = document2.getElementById("KrbIdP").action;
            var useridp = document2.getElementsByName("user_idp");
            var useridpiput = decodeURIComponent((useridp[0].value));
            action = decodeURIComponent(action.replace(host, 'https://wtc.tu-chemnitz.de/'));
            //  document.getElementById("result").innerHTML = action;
            action = decodeURIComponent(action);
            actiontopost = action;
            
            var cookie = (response.cookie);
          //  alert(action);
            _saml_sp = decodeURIComponent(cookie.substring(0, cookie.indexOf(';')) + ";");

            allcookie = allcookie + " " + _saml_sp;

            formid = action.substring(action.lastIndexOf("%") + 1, action.leng);



           Getusername1();

            //   getauthsatet(action, _saml_sp, useridpiput, ShibSessionID);
            //   getauthsatet(action, _saml_sp, idp, ShibSessionID);

            // getshib(action, _saml_sp, useridpiput)


            toastMixin.fire({
                animation: true,
                title: 'Requesting authentication'
            });




        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
}









function Getusername1() {


    $.ajax({
        type: "POST",
        url: "/WTC/Getusername1",
        data: '{url: "' + actiontopost + '", cookie: "' + _saml_sp + '" }',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var cookie = response.cookie;
            var cookie2 = response.cookie2;
            var content = response.content;


            const parser = new DOMParser();



            var arr = cookie.split(";");

            _saml_idp = (decodeURIComponent(arr[0].replace("_saml_idp=", "")));
            //  alert(_saml_idp);
            _saml_sp = (decodeURIComponent(arr[6].replace("HttpOnly,_saml_sp=", "")));
            //alert(_saml_sp);
            _redirect_user_idp = (decodeURIComponent(arr[12].replace("HttpOnly,_redirect_user_idp=", "")));

            newlink = decodeURIComponent(cookie2);



            Shibbolethsso();





        },
        failure: function (response) {
            // document.getElementById("result").innerHTML = response.responseText;
        },
        error: function (response) {
            //  document.getElementById("result").innerHTML = response.responseText;
        }
    });
}

function Shibbolethsso() {

    // document.getElementById("result").innerHTML = authstate;
    $.ajax({
        type: "POST",
        url: "/WTC/Shibbolethsso",
        data: '{url: "' + newlink + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var cookie = response.cookie;
            var cookie2 = response.cookie2;
            var content = response.content;

            const parser = new DOMParser();


            const document2 = parser.parseFromString(response.content, "text/html");
            cookie2 = decodeURIComponent(cookie2);

            cookiereq = (cookie);
            opensamlreq = (cookie);
            opensamlreq = decodeURI(opensamlreq.split(';')[0]);
            opensamlreqname = decodeURI(opensamlreq.split('=')[0]);
            opensamlreqvalue = decodeURI(opensamlreq.split('=')[1]);

            content = decodeURIComponent(content);

            var arr = document2.getElementsByTagName("a");
            newlink = (arr[0].href);

            //toastMixin.fire({
            //    animation: true,
            //    title: 'Getting SamL reuest'
            //});
            SSOServicephp();
        },
        failure: function (response) {
            // document.getElementById("result").innerHTML = response.responseText;
        },
        error: function (response) {
            //  document.getElementById("result").innerHTML = response.responseText;
        }
    });
}


function SSOServicephp() {

    $.ajax({
        type: "POST",
        url: "/WTC/SSOService",
        data: '{url: "' + newlink + '",sp: "' + _saml_sp + '",idp: "' + _saml_idp + '",userdp: "' + _redirect_user_idp + '" }',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var cookie = response.cookie;
            var cookie2 = response.cookie2;
            var content = response.content;

            //  allcookie = cookie;              // make a new parser
            const parser = new DOMParser();


            const document2 = parser.parseFromString(response.content, "text/html");
            cookie2 = decodeURIComponent(cookie2);

            newlink = document2.getElementById("redirlink").href;
            //  alert(cookie);



            usernamephp();
        },
        failure: function (response) {
            // document.getElementById("result").innerHTML = response.responseText;
        },
        error: function (response) {
            //  document.getElementById("result").innerHTML = response.responseText;
        }
    });
}



function usernamephp() {

    $.ajax({
        type: "POST",
        url: "/WTC/Usernanephp",
        data: '{url: "' + newlink + '",sp: "' + _saml_sp + '",idp: "' + _saml_idp + '",userdp: "' + _redirect_user_idp + '" }',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var cookie = response.cookie;
            var cookie2 = response.cookie2;
            var content = response.content;


            const parser = new DOMParser();


            const document2 = parser.parseFromString(response.content, "text/html");
            cookie2 = decodeURIComponent(cookie2);


            var arr = cookie2.split(";");
            ShibSessionID = (arr[0]).replace("ShibSessionID=", "");
            _saml_idp = (arr[1]).replace("_saml_idp=", "");;
            _saml_sp = (arr[2]).replace("_saml_sp=", "");;
            _redirection_state = (arr[3]).replace("_redirection_state=", "");;
            _redirect_user_idp = (arr[4]).replace("_redirect_user_idp=", "");;


            authstate = decodeURIComponent(document2.querySelector('input[name="AuthState"]').value);
            //  alert(authstate);

            Postusername();
        },
        failure: function (response) {
            // document.getElementById("result").innerHTML = response.responseText;
        },
        error: function (response) {
            //  document.getElementById("result").innerHTML = response.responseText;
        }
    });
}



function Postusername() {
   
    $.ajax({
        type: "POST",
        url: "/WTC/Postusername",
        data: '{link:"' + authstate + '",_saml_sp: "' + _saml_sp + '",_saml_idp: "' + _saml_idp + '",_redirect_user_idp: "' + _redirect_user_idp + '",AuthState: "' + authstate + '",ShibSessionID: "' + ShibSessionID + '" }',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var cookie = response.cookie;
            var cookie2 = response.cookie2;
            var content = response.content;

            const parser = new DOMParser();


            const document2 = parser.parseFromString(response.content, "text/html");

            Postpassword();




        },
        failure: function (response) {
            // document.getElementById("result").innerHTML = response.responseText;
        },
        error: function (response) {
            //  document.getElementById("result").innerHTML = response.responseText;
        }
    });
}


function Postpassword() {

    $.ajax({
        type: "POST",
        url: "/WTC/Postpass",
        data: '{link:"' + authstate + '",_saml_sp: "' + _saml_sp + '",_saml_idp: "' + _saml_idp + '",_redirect_user_idp: "' + _redirect_user_idp + '",AuthState: "' + authstate + '",ShibSessionID: "' + ShibSessionID + '" }',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var cookie = response.cookie;
            var cookie2 = response.cookie2;
            var content = response.content;

            //  allcookie = cookie;              // make a new parser
            const parser = new DOMParser();


            const document2 = parser.parseFromString(response.content, "text/html");


            SAMLResponse = decodeURIComponent(document2.querySelector('input[name="SAMLResponse"]').value);
            relaystate = decodeURIComponent(document2.querySelector('input[name="RelayState"]').value);


            postsaml();

        },
        failure: function (response) {
            // document.getElementById("result").innerHTML = response.responseText;
        },
        error: function (response) {
            //  document.getElementById("result").innerHTML = response.responseText;
        }
    });
}

function postsaml() {
    opensamlreqname = decodeURIComponent(opensamlreqname);
    opensamlreqvalue = decodeURIComponent(opensamlreqvalue);
    opensamlreqname = encodeURI(opensamlreqname);



    relaystate = encodeURIComponent(relaystate);
    SAMLResponse = encodeURIComponent(SAMLResponse);
    //  alert(relaystate);
    //  alert(SAMLResponse);

    $.ajax({
        type: "POST",
        url: "/WTC/Saml2post",
        data: '{SAMLResponse:"' + SAMLResponse + '",RelayState: "' + relaystate + '",namee:"' + opensamlreqname + '",vall: "' + opensamlreqvalue + '" }',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var cookie = response.cookie;
            var cookie2 = response.cookie2;
            var content = response.content;


             toastMixin.fire({
        animation: true,
        title: 'Authenticated'
    });
            // document.getElementById("result").innerHTML = (cookie);
          //  cookie = cookie.split(";")[0] ;
           //  alert(cookie);


          //  alert(cookie);

            authenticated = "true";

        },
        failure: function (response) {
            // document.getElementById("result").innerHTML = response.responseText;
        },
        error: function (response) {
            //  document.getElementById("result").innerHTML = response.responseText;
        }
    });
}
