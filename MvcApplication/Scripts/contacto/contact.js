function validacion() {
  let nombre = document.getElementById("name").value;
  let email = document.getElementById("email").value;
  let tema = document.getElementById("subject").value;
  let mensaje = document.getElementById("message").value;


  if (mensaje == undefined) {
    mensaje = "Sin mensaje";
  }

  if (!nombre || !tema || !email) {
    alert("Aún faltan datos para completar el mensaje, por favor revisar! :|");
  } else {
    const mensa =
      "Nombre: " + nombre + "<br/> Tema: " + tema + "<br/> Mensaje: " + mensaje;

    envioCorreo(email, mensa);
  }
}

function envioCorreo(email, mensaje) {
  Email.send({


    Host: "smtp.elasticemail.com",
    Username : "soportevycuz@gmail.com",
    Password: "ecfykdmojjjlpfcn",
    To: 'soportevycuz@gmail.com',
    From: email,
    Subject: "Formulario de contacto VYCUZ",
    Body: mensaje
    }).then((message) => alert(message));
}

