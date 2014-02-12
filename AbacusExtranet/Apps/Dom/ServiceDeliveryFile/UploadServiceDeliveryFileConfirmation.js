var chkAccept, btnNext;

function onChanged(idchk, idbtn) {
    chkAccept = GetElement(idchk);
    btnNext = GetElement(idbtn);

    if (chkAccept.checked == true) {
        btnNext.disabled = false;
    }
    else {
        btnNext.disabled = true;
    }
}