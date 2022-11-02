<?php
// ========================================================================================
// exchange.php v7, Dec 2020
// https://gee.bccr.fi.cr/Indicadores/Suscripciones/WS/wsindicadoreseconomicos.asmx
// Developed by: webmaster.mc@mischunches.com
// ========================================================================================

if (isset($_SESSION['COMPRA']) && isset($_SESSION['VENTA'])) {
	
	$Compra = $_SESSION['COMPRA'];
	$Venta = $_SESSION['VENTA'];

} else {

	//Obtener tipo de cambio
	$doc_c = new DOMDocument();
	$doc_v = new DOMDocument();
	$ind_econom_ws =
	'https://gee.bccr.fi.cr/Indicadores/Suscripciones/WS/wsindicadoreseconomicos.asmx/ObtenerIndicadoresEconomicos';
	$fecha = date("d/m/Y");
	$compra = 317;
	$venta = 318;
	$nombre = 'yourName'; // cambiar por su nombre
	$email = 'youremail@email.com'; // cambiar por su correo electronico
	$tokenBCCR = 'YOURTOKEN'; // cambiar por el Token enviado por el BCCR

	$urlWS_c = $ind_econom_ws."?Indicador=".$compra."&FechaInicio=".$fecha."&FechaFinal=".$fecha."&Nombre=".$nombre.
	"&SubNiveles=N&CorreoElectronico=".$email."&Token=".$tokenBCCR;
	$urlWS_v = $ind_econom_ws."?Indicador=".$venta."&FechaInicio=".$fecha."&FechaFinal=".$fecha."&Nombre=".$nombre.
	"&SubNiveles=N&CorreoElectronico=".$email."&Token=".$tokenBCCR;

	//Valor Compra
	$xml_c = @file_get_contents($urlWS_c);
    if ($xml_c === false) {
       $Compra = "No disponible";
    } else {   
	   $doc_c->loadXML($xml_c);
	   $ind_c = $doc_c->getElementsByTagName('INGC011_CAT_INDICADORECONOMIC')->item(0);
	   $val_c = $ind_c->getElementsByTagName('NUM_VALOR')->item(0);
	   $Compra = substr($val_c->nodeValue,0,-6);
	   $_SESSION['COMPRA'] = $Compra;
    }

	//Valor Venta
	$xml_v = @file_get_contents($urlWS_v);
    if ($xml_v === false) {
       $Venta = "No disponible";
    } else {
        $doc_v->loadXML($xml_v);
        $ind_v = $doc_v->getElementsByTagName('INGC011_CAT_INDICADORECONOMIC')->item(0);
        $val_v = $ind_v->getElementsByTagName('NUM_VALOR')->item(0);
        $Venta = substr($val_v->nodeValue,0,-6);
        $_SESSION['VENTA'] = $Venta;
    }
	
}

?>				