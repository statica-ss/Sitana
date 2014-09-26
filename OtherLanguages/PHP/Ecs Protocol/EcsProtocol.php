<?

require_once('EcsReader.php');
require_once('EcsWriter.php');

function ecsRead(&$data, $offset=0)
{
    if ( substr($data, $offset, 4) != 'ECS1' )
    {
        throw new Exception('Invalid protocol.');
    }
    
    $offset += 4;
    
    $len = mb_strlen($data, '8bit');
    $reader = new EcsReader($data, $offset, $len - $offset);
    
    $struct = $reader->readStructure(0);
    
    return $struct;
}

function ecsWrite($obj)
{
    $writer = new EcsWriter();
    $writer->writeRaw('ECS1');
    
    $writer->writeStructure(0, $obj);
    return $writer->getData();
}

?>