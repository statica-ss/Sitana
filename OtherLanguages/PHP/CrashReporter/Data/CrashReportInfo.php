<?
class CrashReportInfo
{
	public $reportsCount;
	public $deviceGuid;
	public $applicationGuid;

	public function read($reader)
	{
		$this->reportsCount = $reader->readInteger(0);
		$this->deviceGuid = $reader->readString(1);
		$this->applicationGuid = $reader->readString(2);
	}

	public function write($writer)
	{
		$writer->writeValue(0, $this->reportsCount);
		$writer->writeString(1, $this->deviceGuid);
		$writer->writeString(2, $this->applicationGuid);
	}
}
?>