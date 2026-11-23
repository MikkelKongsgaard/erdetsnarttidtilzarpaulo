const assert = require('node:assert/strict');
const fs = require('node:fs');
const vm = require('node:vm');
const source = fs.readFileSync('countdown.js', 'utf8');
const target = Date.parse('2027-11-19T19:00:00Z');
function render(now) {
 const nodes = Object.fromEntries(['days','hours','minutes','seconds','answer','sub'].map(id=>[id,{}]));
 vm.runInNewContext(source, {
  document:{getElementById:id=>nodes[id]},
  Date:class extends Date { static now(){return now;} }, setInterval:()=>{}
 });
 return nodes;
}
assert.equal(render(target-1000).seconds.textContent,'01');
assert.equal(render(target).seconds.textContent,'00');
assert.equal(render(target+86400000).days.textContent,'00');
assert.match(render(target).answer.textContent,/JA, FOR HELVEDE/);
assert.match(render(target-7*86400000).answer.textContent,/NÆRMER/);
assert.match(render(target-30*86400000).answer.textContent,/AFVENTER/);
assert.equal(render(target-(31*86400+3*3600+4*60+5)*1000).hours.textContent,'03');
assert.equal(render(target-(31*86400+3*3600+4*60+5)*1000).minutes.textContent,'04');
const html=fs.readFileSync('screensaver/shared/index.html','utf8');
assert(!/https?:\/\//.test(html));
for(const id of ['days','hours','minutes','seconds','answer','sub']) assert(html.includes(`id="${id}"`));
console.log('Countdown boundaries, timezone instant, and offline asset checks passed.');
const site = fs.readFileSync('index.html','utf8');
assert(site.includes('<script src="countdown.js"></script>'));
assert(site.includes('https://github.com/MikkelKongsgaard/erdetsnarttidtilzarpaulo/releases'));
let shareHandler;
vm.runInNewContext(site.match(/<script>([\s\S]*?)<\/script>/)[1], {
 $:id=>({addEventListener:(event,fn)=>{assert.equal(id,'share');assert.equal(event,'click');shareHandler=fn;}})
});
assert.equal(typeof shareHandler,'function');
console.log('Website script syntax, share registration, and downloads checks passed.');
