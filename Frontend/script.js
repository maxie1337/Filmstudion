function loginPage() {
  const root = document.getElementById('root');
  root.innerHTML = ''; // Clear previous content

  // Create a container
  const container = document.createElement('div');
  container.className = 'container mt-5';

  // Create a card
  const card = document.createElement('div');
  card.className = 'card';
  container.appendChild(card);

  // Create card-body
  const cardBody = document.createElement('div');
  cardBody.className = 'card-body';
  card.appendChild(cardBody);

  // Create error message div (hidden by default)
  const errorDiv = document.createElement('div');
  errorDiv.id = 'loginError';
  errorDiv.className = 'alert alert-danger';
  errorDiv.style.display = 'none';
  cardBody.appendChild(errorDiv);

  // Create login form
  const loginForm = document.createElement('form');
  loginForm.id = 'loginForm';
  loginForm.innerHTML = `
    <h3>Logga in</h3>
    <div class="mb-3">
      <label for="username" class="form-label">Användarnamn</label>
      <input type="text" class="form-control" id="username" placeholder="Ange ditt användarnamn">
    </div>
    <div class="mb-3">
      <label for="password" class="form-label">Lösenord</label>
      <input type="password" class="form-control" id="password" placeholder="Ange ditt lösenord">
    </div>
    <button type="submit" class="btn btn-primary">Logga in</button>
  `;
  cardBody.appendChild(loginForm);

  // Create a div for registration option buttons
  const regButtons = document.createElement('div');
  regButtons.className = 'mt-3';
  regButtons.innerHTML = `
    <button id="btnRegisterAdmin" class="btn btn-secondary me-2">Registrera Administratör</button>
    <button id="btnRegisterStudio" class="btn btn-secondary">Registrera Filmstudio</button>
  `;
  cardBody.appendChild(regButtons);

  // Append container to root
  root.appendChild(container);

  // Login form event listener
  loginForm.addEventListener('submit', function (e) {
    e.preventDefault();
    // Hide any previous error message
    errorDiv.style.display = 'none';
    errorDiv.textContent = '';

    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;

    // Call the authenticate endpoint (adjust the URL as needed)
    fetch('http://localhost:5146/api/users/authenticate', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ Username: username, Password: password })
    })
      .then(response => {
        if (!response.ok) {
          // If not OK, throw an error to be caught below
          throw new Error('Wrong username or password');
        }
        return response.json();
      })
      .then(data => {
        console.log('Login successful:', data);
        // Handle token storage and navigation as needed
      })
      .catch(error => {
        console.error('Login error:', error);
        errorDiv.textContent = error.message;
        errorDiv.style.display = 'block';
      });
  });

  // Event listeners for registration buttons
  document.getElementById('btnRegisterAdmin').addEventListener('click', registerAdmin);
  document.getElementById('btnRegisterStudio').addEventListener('click', registerStudio);
}

function registerAdmin() {
  const root = document.getElementById('root');
  root.innerHTML = ''; // Clear previous content

  const container = document.createElement('div');
  container.className = 'container mt-5';

  const card = document.createElement('div');
  card.className = 'card';
  container.appendChild(card);

  const cardBody = document.createElement('div');
  cardBody.className = 'card-body';
  card.appendChild(cardBody);

  const form = document.createElement('form');
  form.id = 'adminRegisterForm';
  form.innerHTML = `
    <h3>Registrera Administratör</h3>
    <div class="mb-3">
      <label for="adminUsername" class="form-label">Användarnamn</label>
      <input type="text" class="form-control" id="adminUsername" placeholder="Ange användarnamn">
    </div>
    <div class="mb-3">
      <label for="adminPassword" class="form-label">Lösenord</label>
      <input type="password" class="form-control" id="adminPassword" placeholder="Ange lösenord">
    </div>
    <button type="submit" class="btn btn-primary">Registrera Administratör</button>
    <button type="button" id="backToLoginAdmin" class="btn btn-link">Tillbaka</button>
  `;
  cardBody.appendChild(form);
  root.appendChild(container);

  form.addEventListener('submit', function (e) {
    e.preventDefault();
    const username = document.getElementById('adminUsername').value;
    const password = document.getElementById('adminPassword').value;
    // Call the admin registration endpoint
    fetch('http://localhost:5146/api/users/register', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ Username: username, Password: password, IsAdmin: true })
    })
      .then(response => response.json())
      .then(data => {
        console.log('Admin registration successful:', data);
        loginPage(); // Return to login page on success
      })
      .catch(error => console.error('Admin registration error:', error));
  });

  // Back button to return to login page
  document.getElementById('backToLoginAdmin').addEventListener('click', loginPage);
}

function registerStudio() {
  const root = document.getElementById('root');
  root.innerHTML = ''; // Clear previous content

  const container = document.createElement('div');
  container.className = 'container mt-5';

  const card = document.createElement('div');
  card.className = 'card';
  container.appendChild(card);

  const cardBody = document.createElement('div');
  cardBody.className = 'card-body';
  card.appendChild(cardBody);

  const form = document.createElement('form');
  form.id = 'studioRegisterForm';
  form.innerHTML = `
    <h3>Registrera Filmstudio</h3>
    <div class="mb-3">
      <label for="studioName" class="form-label">Namn</label>
      <input type="text" class="form-control" id="studioName" placeholder="Ange namn">
    </div>
    <div class="mb-3">
      <label for="studioCity" class="form-label">Stad</label>
      <input type="text" class="form-control" id="studioCity" placeholder="Ange stad">
    </div>
    <div class="mb-3">
      <label for="studioPassword" class="form-label">Lösenord</label>
      <input type="password" class="form-control" id="studioPassword" placeholder="Ange lösenord">
    </div>
    <button type="submit" class="btn btn-primary">Registrera Filmstudio</button>
    <button type="button" id="backToLoginStudio" class="btn btn-link">Tillbaka</button>
  `;
  cardBody.appendChild(form);
  root.appendChild(container);

  form.addEventListener('submit', function (e) {
    e.preventDefault();
    const name = document.getElementById('studioName').value;
    const city = document.getElementById('studioCity').value;
    const password = document.getElementById('studioPassword').value;
    // Call the film studio registration endpoint
    fetch('http://localhost:5146/api/filmstudio/register', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ Name: name, City: city, Password: password })
    })
      .then(response => response.json())
      .then(data => {
        console.log('Film studio registration successful:', data);
        loginPage(); // Return to login page on success
      })
      .catch(error => console.error('Film studio registration error:', error));
  });

  // Back button to return to login page
  document.getElementById('backToLoginStudio').addEventListener('click', loginPage);
}

// Call loginPage on page load
loginPage();