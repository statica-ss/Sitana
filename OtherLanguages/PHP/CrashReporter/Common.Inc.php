<?
require_once('_Config.php');
require_once('ECS/EcsProtocol.php');

require_once('Data/Application.php');
require_once('Data/Crash.php');
require_once('Data/CrashInstance.php');
require_once('Data/CrashReport.php');
require_once('Data/CrashReportInfo.php');
require_once('Data/DateAndTime.php');
require_once('Data/Device.php');
require_once('Data/Session.php');
require_once('Data/EcsRequest.php');
require_once('Data/EcsResponse.php');

function returnStatus($status, $message, $data=null)
{
    $status = intval($status);
    $response = new EcsResponse($status, $message, $data);
    
    $bin = ecsWrite($response);
    header('Content-Type: application/octet-stream');
    header('Content-Length: ' . mb_strlen($bin, '8bit'));
    header('Content-Disposition: filename=Result.ecs');
    header("Content-Transfer-Encoding: binary");
    
    echo $bin;
    ob_flush();
    flush();
}

function readRequest()
{
    $data = file_get_contents("php://input");
    return ecsRead($data);
}

function connectMySql()
{
    global $MYSQL_HOST;
    global $MYSQL_USER;
    global $MYSQL_PASSWORD;
    global $MYSQL_DB;
    
    $link = mysql_connect($MYSQL_HOST, $MYSQL_USER, $MYSQL_PASSWORD);
    
    if (!$link)
    {
        returnStatus(-1, "Error connecting to database at $MYSQL_HOST");
        exit;
    }
    
    mysql_select_db($MYSQL_DB, $link);
    return $link;
}
?>