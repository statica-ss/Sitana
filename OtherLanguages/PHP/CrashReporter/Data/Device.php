<?
class Device
{
	public $id;
	public $guid;
	public $name;
	public $platform;
	public $osVersion;

	public function read($reader)
	{
		$this->id = $reader->hasField(0) ? $reader->readInteger(0) : null;
		$this->guid = $reader->readString(1);
		$this->name = $reader->readString(2);
		$this->platform = $reader->readString(3);
		$this->osVersion = $reader->readString(4);
	}

	public function write($writer)
	{
		$writer->writeValue(0, $this->id);
		$writer->writeString(1, $this->guid);
		$writer->writeString(2, $this->name);
		$writer->writeString(3, $this->platform);
		$writer->writeString(4, $this->osVersion);
	}
    
    public function insertOrUpdate()
    {
        connectMySql();
        
        $guid = addslashes($this->guid);
        $name = addslashes($this->name);
        $platform = addslashes($this->platform);
        $osVersion = addslashes($this->osVersion);
        
        $result = mysql_query("INSERT INTO Devices (guid,name,platform,osVersion) VALUES ('$guid','$name','$platform','$osVersion') ON DUPLICATE KEY UPDATE guid='$guid', name='$name', platform='$platform', osVersion='$osVersion';");
        mysql_free_result($result);
        
        $this->readFromDb($this->guid);
    }
    
    public function readFromDb($guid)
    {
        connectMySql();
        
        $retValue = false;
        
        $result = mysql_query("SELECT * FROM Devices WHERE guid='$guid';");
        
        if($array = mysql_fetch_assoc($result))
        {
            $this->id = $array['id'];
            $this->name = $array['name'];
            $this->platform = $array['platform'];
            $this->osVersion = $array['osVersion'];
            
            $retValue = true;
        }
        
        mysql_free_result($result);
        return $retValue;
    }
}
?>