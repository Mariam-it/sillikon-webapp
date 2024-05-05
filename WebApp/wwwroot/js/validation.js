

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
        let password = document.getElementsByName(element.dataset.valEqualtoOther.replace("*.", ""))[0].value

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

let forms = document.querySelectorAll('form');
let inputs = forms[0].querySelectorAll('input');
let textArea = forms[0].querySelectorAll('textarea');

textArea.forEach(texArea => {
    if (texArea.type === 'textarea') {
        texArea.addEventListener('keyup', (e) => {
            textareaValidator(e.target)
            console.log('hittamig')
            console.log(e)
        })
    }

})
inputs.forEach(input => {
    if (input.dataset.val === 'true') {
        console.log(textArea)
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
                        console.log(textArea)
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
