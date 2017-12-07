$(initialize);

var animationSpeed = 500;
var delay = 50;
var visited = false;

function initialize() {
    loadTemplates();
    setupNavbar();
}

function disableListeners() {
    $('.logo, .coding, .design, .about, .contact').off("click");
}

function enableListeners() {
    setupNavbar();
}

function loadTemplates() {
    $('#intro').load('templates/main.intro.html');
    $('#coding').load('templates/main.coding.html');
    $('#design').load('templates/main.design.html');
    $('#about').load('templates/main.about.html');
    $('#contact').load('templates/main.contact.html');
}

function setupNavbar() {
    $('.logo').click(function(e) {
        e.preventDefault();
        visitPage('#intro', '.logo');
    });

    $('.coding').click(function(e) {
        e.preventDefault();
        visitPage('#coding', '.coding');
    });

    $('.design').click(function(e) {
        e.preventDefault();
        visitPage('#design', '.design');
    });

    $('.about').click(function(e) {
        e.preventDefault();
        visitPage('#about', '.about');
    });

    $('.contact').click(function(e) {
        e.preventDefault();
        visitPage('#contact', '.contact');
    });
}

function visitPage(page, link) {
    disableListeners();

    if ($(page).hasClass('pt-moveToRightFade'))
    {
        // Move page in
        $(page).addClass('pt-moveFromRightFade');

        // Move other pages out
        $('#intro').addClass('pt-moveToRightFade');
        $('#coding, #design, #about, #contact').addClass('pt-moveToLeftFade');

        // Set the active navbar link
        $('.coding, .design, .about, .contact').removeClass('active');
    }
    else if ($(page).hasClass('pt-moveToLeftFade'))
    {
        // Move page in
        $(page).addClass('pt-moveFromLeftFade');

        // Move other pages out
        $('#intro').addClass('pt-moveToRightFade');
        $('#coding, #design, #about, #contact').not(page).addClass('pt-moveToLeftFade');

        // Set the active navbar link
        $(link).addClass('active');
        $('.coding, .design, .about, .contact').not(link).removeClass('active');
    }
    // Remove animation classes after animation finishes
    setTimeout(function () {
        $(page).removeClass();
        enableListeners();
    }, animationSpeed + delay);

    if (page == '#coding' && !visited)
    {
        playCodingTitle();
    }
}

function playCodingTitle() {
    // Set up carousel
    $('.vgdev_carousel').slick();

    // Toggle the visited bool
    visited = true;

    // Play the animation for first fragment
    setTimeout(function () {
        $('#fragment1').addClass('start');
    }, animationSpeed);

    // Play the animation for second fragment
    setTimeout(function () {
        $('#fragment1').removeClass('start');
        $('#fragment2').addClass('start');
    }, animationSpeed + 2000);

    // Play the animation for final fragment
    setTimeout(function () {
        $('#fragment2').removeClass('start');
        $('#fragment3').addClass('start');
    }, animationSpeed + 4000);
}
