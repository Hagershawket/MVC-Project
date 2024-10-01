// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
//var searchInp = document.getElementById("searchInp");
//searchInp.addEventListener("keyup", function () {

//    var searchValue = searchInp.value;

//    let xhr = new XMLHttpRequest();

//    let url = `https://localhost:7211/Employee/Index?search=${searchValue}`;

//    xhr.open("GET", url, true);

//    xhr.onreadystatechange = function () {
//        if (xhr.readyState == XMLHttpRequest.DONE) { // XMLHttpRequest.DONE == 4
//            if (xhr.status == 200) {
//                document.getElementById("employeeList").innerHTML = xhr.responseText;
//            } else {
//                alert('something else other than 200 was returned');
//            }
//        }
//    };

//    xhr.send();
//})