
    const concertTime = new Date('2027-11-19T20:00:00+01:00').getTime();
    const $ = (id) => document.getElementById(id);
    const two = (n) => String(n).padStart(2, '0');

    function render() {
      const remaining = Math.max(0, concertTime - Date.now());
      const totalSeconds = Math.floor(remaining / 1000);
      const days = Math.floor(totalSeconds / 86400);
      const hours = Math.floor((totalSeconds % 86400) / 3600);
      const minutes = Math.floor((totalSeconds % 3600) / 60);
      const seconds = totalSeconds % 60;

      $('days').textContent = String(days).padStart(2, '0');
      $('hours').textContent = two(hours);
      $('minutes').textContent = two(minutes);
      $('seconds').textContent = two(seconds);

      if (remaining <= 0) {
        $('answer').textContent = 'JA, FOR HELVEDE!';
        $('sub').textContent = 'Koncerttilstand aktiveret. Bevæg dig mod arenaen. 🎸';
        return;
      }

      if (days < 7) {
        $('answer').textContent = 'ALARM: DET ER LIGE OM LIDT';
        $('sub').textContent = 'Systemet registrerer akut nærhed til Zar Paulo.';
      } else if (days < 30) {
        $('answer').textContent = 'STATUS: DET NÆRMER SIG';
        $('sub').textContent = 'Nedtællingsmodulet melder positiv udvikling.';
      } else {
        $('answer').textContent = 'STATUS: AFVENTER ZAR PAULO';
        $('sub').textContent = 'Ventetid registreret. Begejstringen forbliver aktiv.';
      }
    }


render(); setInterval(render,1000);
