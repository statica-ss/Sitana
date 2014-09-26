<?
class EcsResponse
{
	private $status;
    private $data;
    private $message;

    public function __construct($status, $message, $data=null)
    {
        $this->status = $status;
        $this->message = $message;
        $this->data = $data;
    }
    
	public function write($writer)
	{
		$writer->writeValue(0, $this->status);
        $writer->writeString(1, $this->message);
        
        if ( !is_null($this->data) )
        {
            $writer->writeStructure(2, $this->data);
        }
	}
}
?>