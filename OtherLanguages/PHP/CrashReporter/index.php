<?
require_once('Common.Inc.php');

require_once('Actions/ActionDeviceInfo.php');
require_once('Actions/ActionCrashReport.php');
require_once('Actions/ActionStartSession.php');

try
{
    $request = readRequest();
    
    $actionName = $request->getAction();
    
    $action = new $actionName;
    $action->execute($request->getData());
}
catch(Exception $e)
{
    returnStatus(-1, $e->__toString());
}
?>