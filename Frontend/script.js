let currentUser = JSON.parse(localStorage.getItem('currentUser')) || null;

// Är en användare inloggad visas huvudsidan, annars inloggnignssidan
function initApp() {
  if (currentUser) {
    mainPage();
  } else {
    loginPage();
  }
}

// Inloggningssidan, kan regga admin eller filmstudio som får olika roller.
function loginPage() {
  const root = document.getElementById('root');
  root.innerHTML = '';

  const container = document.createElement('div');
  container.className = 'container mt-5';

  const card = document.createElement('div');
  card.className = 'card';
  container.appendChild(card);

  const cardBody = document.createElement('div');
  cardBody.className = 'card-body';
  card.appendChild(cardBody);

  const errorDiv = document.createElement('div');
  errorDiv.id = 'loginError';
  errorDiv.className = 'alert alert-danger';
  errorDiv.style.display = 'none';
  cardBody.appendChild(errorDiv);

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

  const regButtons = document.createElement('div');
  regButtons.className = 'mt-3';
  regButtons.innerHTML = `
    <button id="btnRegisterAdmin" class="btn btn-secondary me-2">Registrera Administratör</button>
    <button id="btnRegisterStudio" class="btn btn-secondary">Registrera Filmstudio</button>
  `;
  cardBody.appendChild(regButtons);

  root.appendChild(container);

  loginForm.addEventListener('submit', function(e) {
    e.preventDefault();
    errorDiv.style.display = 'none';
    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;
    fetch('http://127.0.0.1:5146/api/users/authenticate', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ Username: username, Password: password })
    })
    .then(response => {
      if (!response.ok) throw new Error('Wrong username or password');
      return response.json();
    })
    .then(data => {
      console.log('Login successful:', data);
      currentUser = data;
      localStorage.setItem('currentUser', JSON.stringify(currentUser));
      mainPage();
    })
    .catch(error => {
      console.error('Login error:', error);
      errorDiv.textContent = error.message;
      errorDiv.style.display = 'block';
    });
  });

  document.getElementById('btnRegisterAdmin').addEventListener('click', registerAdmin);
  document.getElementById('btnRegisterStudio').addEventListener('click', registerStudio);
}


// Huvudsidan, som visar filmer, utloggningsknapp

function mainPage() {
  const root = document.getElementById('root');
  root.innerHTML = '';

  // Visa upp inloggad användares namn
  const displayName = currentUser.Name || currentUser.Username || currentUser.username || 'User';

  const navbar = document.createElement('nav');
  navbar.className = 'navbar navbar-expand-lg navbar-light bg-light';
  navbar.innerHTML = `
    <div class="container-fluid">
      <a class="navbar-brand" href="#">SFF Uthyrning</a>
      <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarContent">
        <span class="navbar-toggler-icon"></span>
      </button>
      <div class="collapse navbar-collapse" id="navbarContent">
        <ul class="navbar-nav me-auto mb-2 mb-lg-0">
          <li class="nav-item">
            <a id="rentedMoviesLink" class="nav-link" href="#">Mina hyrda filmer</a>
          </li>
        </ul>
        <span class="navbar-text me-3">Välkommen, ${displayName}</span>
        <button id="logoutBtn" class="btn btn-outline-danger">Logga ut</button>
      </div>
    </div>
  `;
  root.appendChild(navbar);

  // Skapar container för filmkorten
  const movieContainer = document.createElement('div');
  movieContainer.className = 'container mt-4';
  movieContainer.id = 'movieContainer';
  root.appendChild(movieContainer);

  // Hämta filmer från API:et
  fetch('http://127.0.0.1:5146/api/films')
    .then(response => response.json())
    .then(movies => {
      console.log('Fetched movies:', movies);
      if (!movies || movies.length === 0) {
        movieContainer.innerHTML = '<h3>Inga filmer tillgängliga.</h3>';
        return;
      }
      movieContainer.innerHTML = '<h3>Tillgängliga filmer</h3><div class="row row-cols-1 row-cols-md-3 g-4" id="moviesRow"></div>';
      const moviesRow = document.getElementById('moviesRow');

      movies.forEach(movie => {
        const availableCopies = movie.availableCopies !== undefined ? movie.availableCopies : (movie.filmCopies ? movie.filmCopies.length : 0);
        const cardCol = document.createElement('div');
        cardCol.className = 'col';
        const card = document.createElement('div');
        card.className = 'card h-100';

        const imgSrc = movie.imageURL && movie.imageURL.trim() !== '' ? movie.imageURL : 'https://placehold.co/100';
        card.innerHTML = `
          <img src="${imgSrc}" class="card-img-top" alt="${movie.title}">
          <div class="card-body">
            <h5 class="card-title">${movie.title}</h5>
            <p class="card-text">Director: ${movie.director}</p>
            <p class="card-text">Year: ${movie.year}</p>
            <p class="card-text">Copies available: <span id="copies-${movie.filmId}">${availableCopies}</span></p>
          </div>
        `;

        // Om den inloggade användaren är en filmstudio, lägg till "Rent Movie"-knapp
        if (currentUser && (currentUser.Role || currentUser.role) &&
           ( (currentUser.Role && currentUser.Role.toLowerCase() === 'filmstudio') ||
             (currentUser.role && currentUser.role.toLowerCase() === 'filmstudio') )) {
          const rentBtn = document.createElement('button');
          rentBtn.className = 'btn btn-primary m-2';
          rentBtn.textContent = 'Hyr filmen';
          rentBtn.addEventListener('click', function() {
            rentMovie(movie.filmId);
          });
          card.querySelector('.card-body').appendChild(rentBtn);
        }

        cardCol.appendChild(card);
        moviesRow.appendChild(cardCol);
      });
    })
    .catch(error => {
      console.error('Error fetching movies:', error);
      movieContainer.innerHTML = '<h3>Går inte ladda filmer.</h3>';
    });

  document.getElementById('logoutBtn').addEventListener('click', logout);
  document.getElementById('rentedMoviesLink').addEventListener('click', function(e) {
    e.preventDefault();
    showRentedMovies();
  });
}

// Skickar en förfrågan för att hyra en film och uppdaterar UI med nya kopior.
function rentMovie(filmId) {
  const studioId = currentUser.filmStudio ? currentUser.filmStudio.filmStudioId : currentUser.filmStudioId;
  console.log("Studio id:", studioId);
  fetch(`http://127.0.0.1:5146/api/films/rent?id=${filmId}&studioid=${studioId}`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${currentUser.token || currentUser.Token}`
    }
  })
  .then(response => {
    if (!response.ok) throw new Error('Failed to rent movie');
    return response.json();
  })
  .then(data => {
    console.log('Movie rented:', data);
    const copiesSpan = document.getElementById(`copies-${filmId}`);
    let copies = parseInt(copiesSpan.textContent);
    if (copies > 0) {
      copiesSpan.textContent = copies - 1;
    }
  })
  .catch(error => console.error('Error renting movie:', error));
}

// Visar sidan med hyrda filmer för den inloggade filmstudion.
function showRentedMovies() {
  const root = document.getElementById('root');
  root.innerHTML = '';

  const navbar = document.createElement('nav');
  navbar.className = 'navbar navbar-expand-lg navbar-light bg-light';
  navbar.innerHTML = `
    <div class="container-fluid">
      <a class="navbar-brand" href="#">SFF Uthyrning</a>
      <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarContent">
        <span class="navbar-toggler-icon"></span>
      </button>
      <div class="collapse navbar-collapse" id="navbarContent">
        <ul class="navbar-nav me-auto mb-2 mb-lg-0">
          <li class="nav-item">
            <a id="homeLink" class="nav-link" href="#">Alla filmer</a>
          </li>
        </ul>
        <span class="navbar-text me-3">Välkommen, ${currentUser.Name || currentUser.username || 'User'}</span>
        <button id="logoutBtn" class="btn btn-outline-danger">Logga ut</button>
      </div>
    </div>
  `;
  root.appendChild(navbar);

  const rentedContainer = document.createElement('div');
  rentedContainer.className = 'container mt-4';
  root.appendChild(rentedContainer);

  fetch(`http://127.0.0.1:5146/api/mystudio/rentals`, {
    headers: {
      'Authorization': `Bearer ${currentUser.token || currentUser.Token}`
    }
  })
  .then(response => {
    if (!response.ok) {
      throw new Error("Failed to fetch rented movies: " + response.status);
    }
    return response.text();
  })
  .then(text => {
    const rentedMovies = text ? JSON.parse(text) : [];
    rentedContainer.innerHTML = '<h3>Your Rented Movies</h3><div class="row row-cols-1 row-cols-md-3 g-4" id="rentedRow"></div>';
    const rentedRow = document.getElementById('rentedRow');
    rentedMovies.forEach(movie => {
      const cardCol = document.createElement('div');
      cardCol.className = 'col';
      const card = document.createElement('div');
      card.className = 'card h-100';
      const imgSrc = movie.imageURL && movie.imageURL.trim() !== '' ? movie.imageURL : 'https://placehold.co/100';
      card.innerHTML = `
        <img src="${imgSrc}" class="card-img-top" alt="${movie.title}">
        <div class="card-body">
          <h5 class="card-title">${movie.title}</h5>
          <p class="card-text">Director: ${movie.director}</p>
          <p class="card-text">Year: ${movie.year}</p>
        </div>
      `;
      const returnBtn = document.createElement('button');
      returnBtn.className = 'btn btn-warning m-2';
      returnBtn.textContent = 'Return Movie';
      returnBtn.addEventListener('click', function() {
        returnMovie(movie.filmId);
      });
      card.querySelector('.card-body').appendChild(returnBtn);
      cardCol.appendChild(card);
      rentedRow.appendChild(cardCol);
    });
  })
  .catch(error => console.error('Error fetching rented movies:', error));

  document.getElementById('logoutBtn').addEventListener('click', logout);
  document.getElementById('homeLink').addEventListener('click', function(e) {
    e.preventDefault();
    mainPage();
  });
}

// Funktionen skickar en förfrågan om att lämna tillbaka filmen och uppdatera UI.
function returnMovie(filmId) {
  const studioId = currentUser.filmStudio ? currentUser.filmStudio.filmStudioId : currentUser.filmStudioId;
  console.log("Studio id in returnMovie:", studioId);
  
  fetch(`http://127.0.0.1:5146/api/films/return?id=${filmId}&studioid=${studioId}`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${currentUser.token || currentUser.Token}`
    }
  })
  .then(response => {
    if (!response.ok) throw new Error('Failed to return movie');
    return response.text();
  })
  .then(text => {
    const data = text ? JSON.parse(text) : {};
    console.log('Movie returned:', data);
    showRentedMovies();
  })
  .catch(error => console.error('Error returning movie:', error));
}

// Formuläret för att registrera en admin
function registerAdmin() {
  const root = document.getElementById('root');
  root.innerHTML = '';

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
      <label for="adminUsernameReg" class="form-label">Användarnamn</label>
      <input type="text" class="form-control" id="adminUsernameReg" placeholder="Ange användarnamn">
    </div>
    <div class="mb-3">
      <label for="adminPasswordReg" class="form-label">Lösenord</label>
      <input type="password" class="form-control" id="adminPasswordReg" placeholder="Ange lösenord">
    </div>
    <button type="submit" class="btn btn-primary">Registrera Administratör</button>
    <button type="button" id="backToLoginAdmin" class="btn btn-link">Tillbaka</button>
  `;
  cardBody.appendChild(form);
  root.appendChild(container);

  form.addEventListener('submit', function(e) {
    e.preventDefault();
    const username = document.getElementById('adminUsernameReg').value;
    const password = document.getElementById('adminPasswordReg').value;
    fetch('http://127.0.0.1:5146/api/users/register', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ Username: username, Password: password, IsAdmin: true })
    })
    .then(response => {
      if (!response.ok) throw new Error('Registration failed');
      return response.json();
    })
    .then(data => {
      console.log('Admin registration successful:', data);
      loginPage();
    })
    .catch(error => console.error('Admin registration error:', error));
  });

  document.getElementById('backToLoginAdmin').addEventListener('click', loginPage);
}

// Detta är formuläret för att registrera en filmstudio.
function registerStudio() {
  const root = document.getElementById('root');
  root.innerHTML = '';

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

  form.addEventListener('submit', function(e) {
    e.preventDefault();
    const name = document.getElementById('studioName').value;
    const city = document.getElementById('studioCity').value;
    const password = document.getElementById('studioPassword').value;
    fetch('http://127.0.0.1:5146/api/filmstudio/register', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ Name: name, City: city, Password: password })
    })
    .then(response => {
      if (!response.ok) throw new Error('Registration failed');
      return response.json();
    })
    .then(data => {
      console.log('Film studio registration successful:', data);
      loginPage();
    })
    .catch(error => console.error('Film studio registration error:', error));
  });

  document.getElementById('backToLoginStudio').addEventListener('click', loginPage);
}

// Loggar ut användaren, tar bort currentUser och laddar inloggningssidan.
function logout() {
  currentUser = null;
  localStorage.removeItem('currentUser');
  loginPage();
}

initApp();