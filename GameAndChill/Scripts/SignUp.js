regexName = /^[A-Za-z]{2,25}$/;

function testName() {
    var nameValue = $('#name').val();
    var nameError = $('#nameError');

    if (!regexName.test(nameValue)) {

        nameError.html('Invalid!');
        nameError.css('color', 'red');

    }
    else {
        nameError.html('Valid!');
        nameError.css('color','green')
    }
}