document.getElementById('btnRegisterFilmstudio').addEventListener('click', function() {
    document.getElementById('registrationButtons').style.display = 'none';
    document.getElementById('filmstudioRegistrationForm').style.display = 'block';
  });
  document.getElementById('btnRegisterAdmin').addEventListener('click', function() {
  document.getElementById('registrationButtons').style.display = 'none';
  document.getElementById('adminRegistrationForm').style.display = 'block';
  });
  document.getElementById('cancelFilmstudioReg').addEventListener('click', function() {
    document.getElementById('filmstudioRegistrationForm').style.display = 'none';
    document.getElementById('registrationButtons').style.display = 'block';
  });
  document.getElementById('cancelAdminReg').addEventListener('click', function() {
    document.getElementById('adminRegistrationForm').style.display = 'none';
    document.getElementById('registrationButtons').style.display = 'block';
  });

  // Exempel: Hantera inloggning för filmstudio
  document.getElementById('filmstudioLoginForm').addEventListener('submit', function(event) {
    event.preventDefault();
    const username = document.getElementById('filmstudioUsername').value;
    const password = document.getElementById('filmstudioPassword').value;
    fetch('/api/users/authenticate', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ Username: username, Password: password })
    })
    .then(response => response.json())
    .then(data => {
      console.log('Filmstudio inloggning lyckades:', data);
      // Spara token och vidare navigering
    })
    .catch(error => console.error('Fel vid filmstudio inloggning:', error));
  });

  // Exempel: Hantera inloggning för administratör
  document.getElementById('adminLoginForm').addEventListener('submit', function(event) {
    event.preventDefault();
    const username = document.getElementById('adminUsername').value;
    const password = document.getElementById('adminPassword').value;
    fetch('/api/users/authenticate', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ Username: username, Password: password })
    })
    .then(response => response.json())
    .then(data => {
      console.log('Admin inloggning lyckades:', data);
      // Spara token och vidare navigering
    })
    .catch(error => console.error('Fel vid admin inloggning:', error));
  });

  // Exempel: Hantera registrering av Filmstudio
  document.getElementById('filmstudioRegisterForm').addEventListener('submit', function(event) {
    event.preventDefault();
    const name = document.getElementById('filmstudioName').value;
    const city = document.getElementById('filmstudioCity').value;
    const password = document.getElementById('filmstudioRegPassword').value;
    fetch('/api/filmstudio/register', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ Name: name, City: city, Password: password })
    })
    .then(response => response.json())
    .then(data => {
      console.log('Registrering av filmstudio lyckades:', data);
      // Vid lyckad registrering, kanske navigera bort eller visa ett meddelande
    })
    .catch(error => console.error('Fel vid registrering av filmstudio:', error));
  });

  // Exempel: Hantera registrering av Administratör
  document.getElementById('adminRegisterForm').addEventListener('submit', function(event) {
    event.preventDefault();
    const username = document.getElementById('adminUsernameReg').value;
    const password = document.getElementById('adminRegPassword').value;
    fetch('/api/users/register', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ Username: username, Password: password, IsAdmin: true })
    })
    .then(response => response.json())
    .then(data => {
      console.log('Registrering av administratör lyckades:', data);
      // Vid lyckad registrering, kanske navigera bort eller visa ett meddelande
    })
    .catch(error => console.error('Fel vid registrering av administratör:', error));
  });