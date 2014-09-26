<?
class ActionDeviceInfo
{
    public function execute($device)
    {
        if(!($device instanceof Device))
        {
            returnStatus(-1, 'Invalid request data type. Expected: Device.');
            exit;
        }
        
        $device->insertOrUpdate();
        returnStatus(0, 'Device', $device);
        exit;
    }
}
?>