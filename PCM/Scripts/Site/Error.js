function RenderError(code, msg, src) {
    var html = '<div id="div_error" class="row px-3">';
    html += '<div class="alert alert-danger col-12">';
    //Switch case for message
    switch (parseInt(code)) {
        case -1:
            html += '<b>Missing Input (-1):</b><span>The following fields cannot be blank.</span>';
            break;
        case -440:
            html += '<b>Database Error (-440):</b><span>The following feature is non-functional. Please report issue to IT.</span>';
            break;
        case 1000:
            html += '<b>Login Failed:</b><span>The User ID or Password is incorrect. Please check the User ID or Password and try again.</span>';
            break;
        default:
            html += '<b>Warning: Unexpected Error. Please try again. <br />If problem persists, contact IT Support with following error message:</b>';
            html += '<br /><i>Code:' + code + ' --- Message: ' + msg + ' --- Source: ' + src + '</i>';
            break;
    }
    html += '</div></div>';
    return html;
}