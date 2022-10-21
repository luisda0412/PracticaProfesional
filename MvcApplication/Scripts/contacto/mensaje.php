<?php
/*
if(empty($_POST['name']) || empty($_POST['subject']) || empty($_POST['message']) || !filter_var($_POST['email'], FILTER_VALIDATE_EMAIL)) {
  http_response_code(500);
  exit();
}

if(isset($_POST['enviar'])){

    if(!empty($_POST['name']) && !empty($_POST['subject']) && !empty($_POST['message']) && !empty($_POST['email'])){
        $body = "Ha recibido un mail desde su pagina web.\n\n"."Aqui estan los detalles:" . "\r\n"; 
        $name = strip_tags(htmlspecialchars($_POST['name']));
        $email = strip_tags(htmlspecialchars($_POST['email']));
        $subject = strip_tags(htmlspecialchars($_POST['subject']));
        $message = strip_tags(htmlspecialchars($_POST['message']));
        $header= "From:kennethmiranda56@gmail.com" . "\r\n";
        $header.= "Reply-to:kennethmiranda56@gmail.com" . "\r\n";
        $header.= "Z-Mailer: PHP/". phpversion();

        $mail = @mail($body,$email,$name,$subject,$message,$header);
        if($mail){
            echo "<h4>Mail enviado exitosamente!</h4>"
            
        }
        

    }
}*/

?>



