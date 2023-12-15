
const eachCHUNK = 100; // (KB) // the volume of each chunk for uploading // NOTE: the last part may less than 100kb
var start = 0; // first Byte of your file // this varibale will be increase by [chunkSize]
var chunkSize; // (BYTE) // keep the volume of each chunk in BYTE (eachCHUNK * 1024)
var file; // Your file
var continue_or_pause_client; // TRUE: Resuming | False : was Paused
var subProgressValue = 0; // subProgress Global Variable
var allowSubProgressValue; // subProgress Global Variable
var increased_value; // main Progress Global Variable
var current_progress = 0;// main Progress Global Variable
const fileInput = document.getElementById('fileupload1');
var filePartCount; // All parts of files after separating
fileInput.addEventListener('change', handleFileUpload);
// resume section (after re-load)
var resume;
var resume_filename;
var resume_Start;
// end

// fired when file will be choosen
function handleFileUpload(event) {

    if (!resume) {
        continue_or_pause_client = true; // set default value
        file = event.target.files[0]; // get first file from the [input file]
        chunkSize = 1024 * eachCHUNK; // size of each chunk (1MB)
        CalcIncreaseValue(file.size); // [increased value] to progress of progressbar
        filePartCount = Math.ceil(file.size / chunkSize);
        var chunk = file.slice(start, start + chunkSize);
        allowSubProgressValue = true; // SubProgressbar is ready now!
        actionSubProgressbar(); // SubProgressbar is fired!
        uploadChunk(chunk, chunkSize, file.name, filePartCount); // the main function is fired!
        actionProgressbar(true); // the main Progressbar is started!
        $("#btncontinue_or_pause_client").css('visibility', 'visible');
        $("#BoxSubProgress").css('visibility', 'visible');
    } else { // there is a file for resuming ...
        file = event.target.files[0];
        if (file.name != resume_filename) {
            alert('The selected file is not the exact file.');
            return; // exit function!
        } else {
            //alert(resume_Start);
            //return;
            continue_or_pause_client = true; // set default value
            file = event.target.files[0]; // get first file from the [input file]
            chunkSize = 1024 * eachCHUNK; // size of each chunk (1MB)
            CalcIncreaseValue(file.size); // [increased value] to progress of progressbar
            current_progress = CalcLeftResumeValue(file.size);
            filePartCount = Math.ceil(file.size / chunkSize);
            //=============================================
            // The following is the most important, because when we stop the upload process
            // the last START point will be stored, and after fetching again, we have to increase its value
            // to get the next point
            resume_Start = parseInt(resume_Start) + chunkSize;
            var chunk = file.slice(parseInt(resume_Start), parseInt(resume_Start) + chunkSize);
            start = parseInt(resume_Start);
            //=============================================
            allowSubProgressValue = true; // SubProgressbar is ready now!
            actionSubProgressbar(); // SubProgressbar is fired!
            uploadChunk(chunk, chunkSize, file.name, filePartCount); // the main function is fired!
            actionProgressbar(true); // the main Progressbar is started!
            $("#btncontinue_or_pause_client").css('visibility', 'visible');
            $("#BoxSubProgress").css('visibility', 'visible');
            $("#btncontinue_or_pause_client").html('Pause');
            $(".progress").css('background-image', '')
        }
    }
}

// main function
var timestamp = new Date().getTime();

function uploadChunk(chunk, chunkSize, filename, filePartCount) {


    if (continue_or_pause_client) {
        var postData = new FormData();
        postData.append("file", chunk);
        postData.append("ChunkSize", chunkSize);
        postData.append("Filename", filename);
        postData.append("Start", start);
        postData.append("FilePartCount", filePartCount);

        // calc time and speed of upload
        var startTime = (new Date()).getTime();
        var endTime = startTime;
        // end

        $.ajax({
            contentType: false,
            processData: false,
            type: 'POST',
            data: postData,
            url: 'Home/Upload',
            success: function (data) {
                if (data.success) {

                    //////////// Time & Speed of Uploading
                    endTime = (new Date()).getTime();
                    var time = (endTime - startTime) / 1000;
                    var sizeInByte = chunkSize * 8;
                    var speed = ((sizeInByte / time) / (1024 * 1024)).toFixed(2);
                    $("#idBoxbar").html("File: <span class='idBoxbarSpan'>" + filename + "</span> | Time: <span class='idBoxbarSpan'>" + time + "</span> [MS] | <span class='idBoxbarSpan'>Speed: " + speed + "</span> [MPS]");
                    //////////// End

                    start += chunkSize; // chunkSize is not interval, is an index!
                    var chunk2 = file.slice(start, start + chunkSize);

                    if (chunk2.size >= chunkSize) {
                        subProgressValue = 0;
                        actionProgressbar(true);
                    }
                    else {
                        chunkSize = chunkSize - chunk2.size;
                        chunk2 = file.slice(start, start + chunkSize);
                        actionProgressbar(false); // if the last part is less than [eachCHUNK] KB
                    }
                    if (start < file.size && continue_or_pause_client)
                        uploadChunk(chunk2, chunkSize, file.name, filePartCount);
                }
            },
            error: function (request, status, error) {
                //alert('request:' + request.responseText + ';err:' + error);
            }
        });
    }
}

// pausing
function pause() {
    continue_or_pause_client = !continue_or_pause_client;
    if (!continue_or_pause_client) {
        $("#dynamic").removeClass("progress-bar-success");
        $("#dynamic").addClass("progress-bar-success-gray");
        $("#btncontinue_or_pause_client").html('Resume');
    }
    else {
        $("#btncontinue_or_pause_client").html('Pause');
        $("#dynamic").removeClass("progress-bar-success-gray");
        $("#dynamic").addClass("progress-bar-success");

        var chunk = file.slice(start, start + chunkSize);
        uploadChunk(chunk, chunkSize, file.name, filePartCount);
    }
}

// Merging
function merging() {
    $.ajax({
        contentType: false,
        dataType: false,
        type: 'POST',
        url: 'Home/Merge',
        success: function () {
            //alert('1');
        },
        error: function (request, status, error) {
            //alert('request:' + request.responseText + ';err:' + error);
        }
    });
}

// main progressbar
function actionProgressbar(lastPartState) {
    if (lastPartState) {
        current_progress += increased_value;
        var showedValue = parseFloat(current_progress.toFixed(2))
        $("#dynamic")
            .css("width", current_progress + "%")
            .attr("aria-valuenow", current_progress);
        //.text(showedValue + "%");
        $("#idMainBoxbar").text(showedValue + "%");
    }
    else {
        $("#dynamic")
            .css("width", 100 + "%")
            .attr("aria-valuenow", 100);
        //.text(100 + "%");
        $("#idMainBoxbar").text("100%");
        allowSubProgressValue = false;
        $("#btnMerge").css('visibility', 'visible');
        $("#btncontinue_or_pause_client").css('visibility', 'hidden'); // Hide [pause-continue botton] after being completed the upload process (100%)
        $("#BoxSubProgress").css('visibility', 'hidden');
    }
}

// CalcIncreaseValue
function CalcIncreaseValue(fileSize) {
    var kb = fileSize / 1024; // convert to KB
    var eachKb = Math.ceil(kb / eachCHUNK);
    var eachUnit = 100 / eachKb;
    increased_value = eachUnit;
}

// CalcLeftResumeValue
function CalcLeftResumeValue(fileSize) {
    var kb = fileSize / 1024; // convert to KB
    var eachKb = Math.ceil(kb / eachCHUNK);
    var kb_resume = parseInt(resume_Start) / 1024;
    return (kb_resume / eachKb);
}

// main Sub Progressbar
function actionSubProgressbar() {
    var elem = document.getElementById("myBar");
    var id = setInterval(frame, 10);
    function frame() {
        if (allowSubProgressValue) {
            if (subProgressValue >= 100) {
                clearInterval(id);
                subProgressValue = 0;
                actionSubProgressbar();
            } else {
                subProgressValue++;
                elem.style.width = subProgressValue + '%';
            }
        } else {
            elem.style.width = 0 + '%';
            elem.innerHTML = '';
        }
    }
}

// load page and resume
$(document).ready(function () {

    // check if the last film is uploaded or not
    // the filename must be getting with a parameter such as querystring
    // but here we assume the left file is "test.jpg"

    var postData = { filename: 'test.jpg' };
    $.ajax({
        contentType: 'application/x-www-form-urlencoded',
        dataType: 'json',
        type: 'POST',
        data: postData,
        url: 'Home/CheckResume',
        success: function (data) {
            if (data.resume) {
                //
                resume = true;
                resume_filename = data.filename;
                resume_Start = data.start;
                ///
                $("#dynamic").removeClass("progress-bar-success-gray");
                $("#dynamic").addClass("progress-bar-success");
                $("#btncontinue_or_pause_client").html('Resume');
                $("#BoxSubProgress").css('visibility', 'visible');
                $("#idMainBoxbar").text("click to continue");
                $(".progress")
                    .css('background-image', 'linear-gradient(57deg, #808080 27.59%, #939393 27.59%, #939393 50%, #808080 50%, #808080 77.59%, #939393 77.59%, #939393 100%)')
                    .css('background-size', '28.62px 44.07px');

            }
        },
        error: function (req, res, err) {
            alert('req' + req.responseText + ' err:' + err);
        },
    });

});