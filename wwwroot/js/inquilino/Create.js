async function verificarDni(event) {
            event.preventDefault();
            const dni = document.getElementById('dniVerificar').value.trim();
            if (!dni) return false;

            // Llamada real al backend
            let date = null;
            try {
                const response = await fetch(`/Inquilino/ExisteDni?dni=${encodeURIComponent(dni)}`);
                date = await response.json();
            } catch (e) {
                document.getElementById('dniError').innerText = 'Error al verificar el DNI.';
                return false;
            }
            if (date) {
                document.getElementById('dniMensaje').innerText = date.mensaje;
                return false;
            } else {
                document.getElementById('dniMensaje').innerText = '';
                document.getElementById('altaForm').style.display = '';
                document.getElementById('dniForm').style.display = 'none';
                document.getElementById('Dni').value = dni;
                document.getElementById('Dni').readOnly = true;
            }
            return false;
        }