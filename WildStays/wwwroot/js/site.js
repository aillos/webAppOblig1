document.addEventListener("DOMContentLoaded", function () {
    const searchButton = document.getElementById("searchButton");
    const textInput = document.getElementById("search");
    const additionalInput1 = document.getElementById("additional1");
    const additionalInput2 = document.getElementById("additional2");
    const label1 = document.getElementById("label1");
    const label2 = document.getElementById("label2");

    searchButton.addEventListener('click', () => {
      
        if (textInput.value.length > 0) {

            label1.style.opacity = "1";
            label2.style.opacity = "1";
            additionalInput1.style.opacity = "1";
            additionalInput2.style.opacity = "1";
        } else {

            additionalInput1.style.opacity = "0";
            additionalInput2.style.opacity = "0";
            label1.style.opacity = "0";
            label2.style.opacity = "0";
        }
    });
});
