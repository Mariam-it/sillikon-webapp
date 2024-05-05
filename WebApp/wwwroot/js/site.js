function adjustFooterPosition() {
    var viewHeight = window.innerHeight;
    var body = document.querySelector('body');
    var footer = document.querySelector('footer');

    if (viewHeight >= body.offsetHeight) {
        footer.style.position = 'fixed';
        footer.style.bottom = '0';
        footer.style.right = '0';
        footer.style.left = '0';
    } else {
        footer.style.position = '';
        footer.style.bottom = '';
        footer.style.right = '';
        footer.style.left = '';
    }

}

// Anropa funktionen när sidan laddas och när fönstret ändrar storlek
window.addEventListener('DOMContentLoaded', adjustFooterPosition);
window.addEventListener('resize', adjustFooterPosition);


document.addEventListener('DOMContentLoaded', function () {
    const navToggle = document.querySelector('.nav-toggle');
    const menu = document.querySelector('.menu');

    navToggle.addEventListener('click', function () {

        menu.classList.toggle('is-active');

        menu.style.display = menu.style.display === 'flex' ? 'none' : 'flex';
    });
});

document.querySelector('input[type="checkbox"]').addEventListener('change', function () {
    if (this.checked) {
        this.parentNode.style.backgroundColor = '#6366F1';
    } else {
        this.parentNode.style.backgroundColor = '#565973';
    }
});

document.addEventListener('DOMContentLoaded', function () {
    let sw = document.querySelector('#switch-mode');

    sw.addEventListener('change', function () {
        let theme = this.checked ? "dark" : "light";

        fetch(`/sitesettings/changetheme?theme=${theme}`)
            .then(res => {
                if (res.ok)
                    window.location.reload()
                else
                    console.log("wrong theme mode")
            })
    })
})

document.addEventListener('DOMContentLoaded', function () {
    handleProfileImageUpload()

})

function handleProfileImageUpload() {
    try {
        let fileUploader = document.querySelector('#fileUploader')
        if (fileUploader != undefined) {
            fileUploader.addEventListener('change', function () {
                if (this.form.length > 0) {
                    this.form.submit()
                }
            })
        }
    }
    catch { }
}

document.addEventListener('DOMContentLoaded', function () {
    select()
})

function select() {
    try {
        let select = document.querySelector('.select')
        let selected = select.querySelector('.selected')
        let selectOptions = select.querySelector('.select-options')

        selected.addEventListener('click', function () {
            selectOptions.style.display = (selectOptions.style.display === 'block') ? 'none' : 'block'
        })

        let options = selectOptions.querySelectorAll('.option')

        options.forEach(function (option) {
            option.addEventListener('click', function () {
                selected.innerHTML = this.textContent
                selectOptions.style.display = 'none'
                let category = this.getAttribute('data-value')
                selected.setAttribute('data-value', category)
                updateCoursesByFilter()
            })
        })
    }
    catch { }
}

function searchQuery() {
    try {
        document.querySelector('#searchQuery').addEventListener('keyup', function () {
            updateCoursesByFilter()
        })
    }
    catch { }
}
function updateCoursesByFilter() {

    const category = document.querySelector('.select .selected').getAttribute('data-value') || 'all'
    const searchQuery = document.querySelector('#searchQuery').value

    const url = `/courses?category=${encodeURIComponent(category)}&searchQuery=${encodeURIComponent(searchQuery)}`

    fetch(url)
        .then(res => res.text())
        .then(data => {
            const parser = new DOMParser()
            const dom = parser.parseFromString(data, 'text/html')
            document.querySelector('.items').innerHTML = dom.querySelector('.items').innerHTML

            const pagination = dom.querySelector('.courses-pagenation') ? dom.querySelector('.courses-pagenation').innerHTML : ''
            document.querySelector('.courses-pagenation').innerHTML = pagination
        })
}




// Function to toggle the display of elements and highlight active element
function toggleDisplayAndHighlight(event) {
    // Remove 'active' class from all profile-nav items
    document.querySelectorAll('.profile-nav').forEach((navItem) => {
        navItem.classList.remove('active');
    });

    // Add 'active' class to the clicked profile-nav item
    event.target.classList.add('active');

    // Assuming you have a detail section with a class that needs to be shown/hidden
    const detailSections = document.querySelectorAll('.detail-section');

    // Hide all detail sections
    detailSections.forEach(section => {
        section.style.display = 'none';
    });

    // Logic to determine which detail section to show
    // For example, using data-attributes to associate menu items with detail sections
    const sectionIdToShow = event.target.getAttribute('data-section-id');
    const sectionToShow = document.querySelector(`#${sectionIdToShow}`);
    if (sectionToShow) {
        sectionToShow.style.display = 'grid'; // Or 'block' if it's not a grid layout
    }
}

// Attach the event listener to each profile-nav item
document.querySelectorAll('.profile-nav').forEach((navItem) => {
    navItem.addEventListener('click', toggleDisplayAndHighlight);
});


