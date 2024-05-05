
let formsToValidate = [
    document.querySelectorAll('form')[1],
    document.querySelectorAll('form')[2],
    document.querySelectorAll('form')[3],
    document.querySelectorAll('form')[4]
];



let myInputs = [];

// Loopa igenom varje formulär och hämta dess inmatningsfält
formsToValidate.forEach(form => {
    const inputs = form.querySelectorAll('input, textarea');
    myInputs.push(...inputs);
});


const formErrorHandler = (element, validationResult) => {
    let spanElement = document.querySelector(`[data-valmsg-for="${element.name}"]`);

    if (validationResult) {
        element.classList.remove('input-validation-error');
        spanElement.classList.remove('field-validation-error');
        spanElement.classList.remove('field-validation-valid');
        spanElement.innerHTML = '';
    } else {
        element.classList.add('input-validation-error');
        spanElement.classList.add('field-validation-error');
        spanElement.classList.remove('field-validation-valid');
        spanElement.innerHTML = element.dataset.valRequired
    }
}

const textValidator = (element, minLength = 2) => {
    if (element.value.length >= minLength) {
        console.log("jag hittas")
        formErrorHandler(element, true);
    }
    else
        formErrorHandler(element, false);
}

const textareaValidator = (element, minLength = 10) => {
    if (element.value.length >= minLength) {
        console.log("jag hittas")
        formErrorHandler(element, true);
    }
    else
        formErrorHandler(element, false);
}

const emailValidator = (element) => {
    const regEx = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|.(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    formErrorHandler(element, regEx.test(element.value));
}

const passwordValidator = (element) => {
    if (element.dataset.valEqualtoOther !== undefined) {
        let password = document.getElementsByName(element.dataset.valEqualtoOther.replace("*", "ChangePassword"))[0].value

        if (element.value === password)
            formErrorHandler(element, true);
        else
            formErrorHandler(element, false);
    } else {
        const regEx = /^(?=.*[0-9])(?=.*[!@#$%^&*])[a-zA-Z0-9!@#$%^&*]{6,16}$/;
        formErrorHandler(element, regEx.test(element.value));
    }
}

const checkboxValidator = (element) => {
    formErrorHandler(element, element.checked);
}

myInputs.forEach(input => {
    if (input.dataset.val === 'true') {
        if (input.type === 'checkbox') {
            console.log(input)
            input.addEventListener('change', (e) => {
                checkboxValidator(e.target);
            });
        } else {
            input.addEventListener('keyup', (e) => {
                switch (e.target.type) {
                    case 'text':
                        textValidator(e.target);
                        break;
                    case 'email':
                        emailValidator(e.target);
                        break;
                    case 'password':
                        passwordValidator(e.target);
                        break;
                }
            });
        }
    }
});




// delete account validation
function confirmDelete() {
    const deleteCheckbox = document.getElementById('delete-account');
    if (deleteCheckbox.checked) {
        // Visa ett bekräftelsemeddelande
        const confirmation = confirm("Are you sure you want to delete your account?");
        if (confirmation) {
            // Om användaren bekräftar, skicka formuläret
            document.getElementById('delete-account-form').submit();
        } else {
            // Om användaren inte bekräftar, gör inget
            return false;
        }
    } else {
        // Om rutan inte är markerad, lägg till en klass för att indikera ett valideringsfel
        deleteCheckbox.classList.add('input-validation-error');
        return false;
    }
}
