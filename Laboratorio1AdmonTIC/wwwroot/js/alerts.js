(function($) {
    showSwal = function (type, nombre, deleteUrl) {
        if (type === 'confirm-delete') {
            swal({
                title: "¿Está seguro?",
                text: `¿Desea eliminar el registro "${nombre}"?`,
                icon: "warning",
                buttons: {
                    cancel: {
                        text: "Cancelar",
                        visible: true,
                        className: "btn btn-secondary",
                        closeModal: true
                    },
                    confirm: {
                        text: "Sí, eliminar",
                        visible: true,
                        className: "btn btn-danger",
                        closeModal: true
                    }
                }
            }).then((willDelete) => {
                if (willDelete) {
                    window.location.href = deleteUrl;
                }
            });
        }
        else if (type === 'title-and-text') {
      swal({
        title: 'Read the alert!',
        text: 'Click OK to close this alert',
        button: {
          text: "OK",
          value: true,
          visible: true,
          className: "btn btn-primary"
        }
      })

    } else if (type === 'success-message') {
      swal({
        title: 'Congratulations!',
        text: 'You entered the correct answer',
        icon: 'success',
        button: {
          text: "Continue",
          value: true,
          visible: true,
          className: "btn btn-primary"
        }
      })

    } else if (type === 'auto-close') {
      swal({
        title: 'Auto close alert!',
        text: 'I will close in 2 seconds.',
        timer: 2000,
        button: false
      }).then(
        function() {},
        // handling the promise rejection
        function(dismiss) {
          if (dismiss === 'timer') {
            console.log('I was closed by the timer')
          }
        }
      )
    } else if (type === 'warning-message-and-cancel') {
      swal({
        title: 'Estas seguro?',
        text: `¿Desea eliminar el registro "${nombre}"?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3f51b5',
        cancelButtonColor: '#ff4081',
        confirmButtonText: 'Great ',
        buttons: {
          cancel: {
            text: "Cancel",
            value: null,
            visible: true,
            className: "btn btn-danger",
            closeModal: true,
          },
          confirm: {
            text: "OK",
            value: true,
            visible: true,
            className: "btn btn-primary",
            closeModal: true
          }
        }
      })

    } else if (type === 'custom-html') {
      swal({
        content: {
          element: "input",
          attributes: {
            placeholder: "Type your password",
            type: "password",
            class: 'form-control'
          },
        },
        button: {
          text: "OK",
          value: true,
          visible: true,
          className: "btn btn-primary"
        }
      })
    }
  }

})(jQuery);
function confirmarEliminacion(event, url) {
    event.preventDefault(); // evitar navegación directa

    Swal.fire({
        title: '¿Estás seguro?',
        text: 'Esta acción eliminará el registro permanentemente.',
        icon: 'warning',
        background: '#222437',    
        color: '#adadb8',
        customClass: {
            icon: 'mt-5 mb-0',
            confirmButton: 'btn btn-inverse-danger btn-fw',
            cancelButton: 'btn btn-inverse-secondary btn-fw'
        },
        showCancelButton: true,
        confirmButtonText: 'Eliminar',
        cancelButtonText: 'Cancelar',
        //input: 'password',  // Campo para ingresar la contraseña
        
        
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.href = url;
        }
    });
}

function mostrarErrorDesdeTempData(mensaje) {
    if (mensaje && mensaje !== '') {
        Swal.fire({
            title: 'Advertencia',
            text: mensaje,
            footer: '¡No fue posible ejecutar la acción!',
            icon: 'info',
            background: '#222437',
            color: '#adadb8',
            confirmButtonText: 'Entendido',
            customClass: {
                icon: 'mt-5 mb-0',
                confirmButton: 'btn btn-inverse-info btn-fw'
            }
        });
    }
}

function errorLogin(mensaje) {
    if (mensaje && mensaje !== '') {
        Swal.fire({
            icon: 'error',
            title: 'Login inválido',
            text: mensaje,
            background: '#222437',
            color: '#adadb8',
            confirmButtonText: 'Entendido',
            customClass: {
                icon: 'mt-5 mb-0',
                confirmButton: 'btn btn-inverse-danger btn-fw'
            }
        });
    }
    
}

