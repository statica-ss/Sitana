<?
class EcsRequest
{
	private $action;
	private $data;

	public function read($reader)
	{
		$this->action = $reader->readString(0);
        
        if($reader->hasField(1))
        {
            $this->data = $reader->readStructure(1);
        }
	}
    
    public function getAction()
    {
        return $this->action;
    }
    
    public function getData()
    {
        return $this->data;
    }
}
?>