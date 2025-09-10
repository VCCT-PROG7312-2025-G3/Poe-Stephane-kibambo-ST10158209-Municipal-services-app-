(function () {
    function update() {
        var loc = (document.getElementById('Location')?.value || '').trim().length > 0;
        var cat = (document.getElementById('Category')?.value || '').length > 0;
        var descText = (document.getElementById('Description')?.value || '').trim();
        var desc = descText.length >= 10;
        var att = (document.getElementById('Attachment')?.value || '').length > 0;

        var score = 0;
        if (loc) score += 25;
        if (cat) score += 25;
        if (desc) score += 25;
        if (att) score += 25;

        var bar = document.getElementById('engagementBar');
        var pct = document.getElementById('engagementPercent');
        if (bar && pct) {
            bar.style.width = score + '%';
            bar.setAttribute('aria-valuenow', String(score));
            pct.textContent = score + '%';
        }

        var msg = 'Start by entering a location.';
        if (loc && !cat) msg = 'Great start — now choose a category.';
        else if (loc && cat && !desc) msg = 'Nice — add a short description (10+ characters).';
        else if (loc && cat && desc && !att) msg = 'Optional: attach a photo or PDF for faster resolution.';
        else if (score === 100) msg = 'Awesome! You are ready to submit.';

        var el = document.getElementById('engagementMsg');
        if (el) el.textContent = msg;

        // checkmarks
        function vis(id, on) {
            var i = document.getElementById(id);
            if (i) i.style.visibility = on ? 'visible' : 'hidden';
        }
        vis('chkLoc', loc);
        vis('chkCat', cat);
        vis('chkDesc', desc);
        vis('chkAtt', att);
    }

    ['Location', 'Category', 'Description', 'Attachment'].forEach(function (id) {
        var el = document.getElementById(id);
        if (el) el.addEventListener('input', update);
        if (el) el.addEventListener('change', update);
    });

    // Initial call
    update();
})();
