const $ = (s) => document.querySelector(s),
  labels = { approved: "GENEHMIGT", rejected: "ABGELEHNT", quarantine: "QUARANTÄNE" };
let cases = [];
function render() {
  const query = $("#search").value.toLowerCase();
  const filtered = cases.filter((c) =>
    (c.applicant.name + " " + c.applicant.planet).toLowerCase().includes(query),
  );
  $("#application-list").replaceChildren();
  $("#empty").hidden = filtered.length > 0;
  $("#empty").textContent = cases.length
    ? "Keine Akten für diese Suche."
    : "Keine Einreisen registriert. Noch ist die Erde unter sich.";
  for (const c of filtered) {
    const row = document.createElement("tr");
    row.dataset.id = c.id;
    for (const value of [
      "E-" + String(c.id).padStart(4, "0"),
      c.applicant.name,
      c.applicant.planet,
      c.applicant.days + " Tage",
      labels[c.decision.status],
    ]) {
      const td = document.createElement("td");
      td.textContent = value;
      row.append(td);
    }
    row.lastChild.className = "badge " + c.decision.status;
    row.lastChild.title = c.decision.reason;
    $("#application-list").append(row);
  }
}
async function load() {
  try {
    const r = await fetch("/api/cases");
    if (!r.ok) throw Error("Register nicht erreichbar. Bitte erneut versuchen.");
    cases = await r.json();
    render();
    $("#register-error").textContent = "";
  } catch (e) {
    $("#register-error").textContent = e.message;
  }
}
const portraitIds = ["zorg", "nox", "spora", "lexa", "kira", "borin"];
const samples = [
  {
    name: "Zorg Blubber III",
    planet: "Schleimora",
    purpose: "Tourismus",
    days: 5,
    blackHole: false,
    fungus: false,
    shapeshifter: false,
    validIdentity: true,
    quote: "Ich bin nur wegen der Bratwurst hier.",
  },
  {
    name: "Nox Taschenuniversum",
    planet: "Orion",
    purpose: "Diplomatie",
    days: 3,
    blackHole: true,
    fungus: false,
    shapeshifter: false,
    validIdentity: true,
    quote: "Das schwarze Loch? Das ist mein Handgepäck.",
  },
  {
    name: "Spora Flausch",
    planet: "Kepler-186f",
    purpose: "Tourismus",
    days: 12,
    blackHole: false,
    fungus: true,
    shapeshifter: false,
    validIdentity: true,
    quote: "Das leuchtende Moos gehört normalerweise nicht zu mir.",
  },
  {
    name: "Lexa Vielgesicht",
    planet: "Orion",
    purpose: "Tourismus",
    days: 30,
    blackHole: false,
    fungus: false,
    shapeshifter: true,
    validIdentity: true,
    quote: "Das Passfoto stimmt. Ich brauche nur einen Moment.",
  },
  {
    name: "Kira Lichtleiter",
    planet: "Kepler-186f",
    purpose: "Arbeit",
    days: 90,
    blackHole: false,
    fungus: false,
    shapeshifter: false,
    validIdentity: true,
    quote:
      "Ich installiere das neue Kommunikationsnetz. Diesmal mit Empfang im ganzen Sonnensystem.",
  },
  {
    name: "Borin Schaltkreis",
    planet: "Orion",
    purpose: "Arbeit",
    days: 180,
    blackHole: false,
    fungus: false,
    shapeshifter: false,
    validIdentity: true,
    quote: "Ich warte Ihre Raumfähren. Vier Augen sehen jeden Wackelkontakt.",
  },
];
const securityFields = {
  blackHole: "Schwarzes Loch",
  fungus: "Weltraumpilz",
  shapeshifter: "Formwandler",
  validIdentity: "Gültiger Identitätsnachweis",
};
for (const [i, profile] of samples.entries()) {
  const card = document.createElement("article");
  card.className = "sample-card";
  card.hidden = i !== 0;
  const number = document.createElement("p");
  number.className = "eyebrow";
  number.textContent = "AKTE " + String(i + 1).padStart(2, "0");
  const title = document.createElement("h3");
  title.textContent = profile.name;
  const info = document.createElement("p");
  info.textContent = profile.planet + " · " + profile.purpose + " · " + profile.days + " Tage";
  const quote = document.createElement("blockquote");
  quote.textContent = "«" + profile.quote + "»";
  const dl = document.createElement("dl");
  for (const [key, label] of Object.entries(securityFields)) {
    const dt = document.createElement("dt"),
      dd = document.createElement("dd");
    dt.textContent = label;
    dd.textContent = profile[key] ? "Ja" : "Nein";
    dl.append(dt, dd);
  }
  const button = document.createElement("button");
  button.type = "button";
  button.className = "secondary";
  button.id = i === 0 ? "sample" : "sample-" + i;
  button.textContent = "Akte übernehmen ↗";
  button.setAttribute("aria-label", profile.name + ": Akte übernehmen");
  button.onclick = () => {
    for (const key of ["name", "planet", "purpose", "days"]) $("#" + key).value = profile[key];
    for (const key of Object.keys(securityFields)) $("#" + key).checked = profile[key];
    $("#result").hidden = true;
    delete $("#result").dataset.decision;
    delete $("#result").dataset.applicationId;
    document.querySelectorAll(".sample-card").forEach((c) => c.classList.remove("selected"));
    card.classList.add("selected");
    $("#name").focus();
  };
  card.append(number, title, info, quote, dl, button);
  $("#sample-cards").append(card);
}
let activeSample = 0;
function showSample(index) {
  activeSample = (index + samples.length) % samples.length;
  document.querySelectorAll(".sample-card").forEach((card, i) => {
    card.hidden = i !== activeSample;
  });
  const profile = samples[activeSample];
  $("#sample-portrait").src = "assets/" + portraitIds[activeSample] + ".png";
  $("#sample-portrait").alt = "Porträt von " + profile.name;
  $("#sample-photo-label").textContent =
    "BIOMETRISCHE ERFASSUNG / AKTE " + String(activeSample + 1).padStart(2, "0");
  $("#sample-position").textContent =
    activeSample + 1 + " / " + samples.length + " · " + profile.name;
}
$("#sample-prev").onclick = () => showSample(activeSample - 1);
$("#sample-next").onclick = () => showSample(activeSample + 1);
showSample(0);
$("#search").oninput = render;
$("#refresh").onclick = load;
$("#application-form").onsubmit = async (e) => {
  e.preventDefault();
  const button = $("#submit"),
    result = $("#result");
  button.disabled = true;
  button.textContent = "AKTE WIRD GEPRÜFT …";
  result.hidden = true;
  try {
    const data = {
      name: $("#name").value,
      planet: $("#planet").value,
      purpose: $("#purpose").value,
      days: Number($("#days").value),
    };
    for (const id of ["blackHole", "fungus", "shapeshifter", "validIdentity"])
      data[id] = $("#" + id).checked;
    const r = await fetch("/api/cases", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    });
    const c = await r.json();
    if (!r.ok) throw Error(c.error || "Prüfung fehlgeschlagen.");
    result.dataset.decision = c.decision.status;
    result.dataset.applicationId = c.id;
    result.replaceChildren();
    const title = document.createElement("b");
    title.textContent = labels[c.decision.status];
    result.append(title, document.createTextNode(c.decision.reason));
    await load();
  } catch (e) {
    delete result.dataset.decision;
    result.textContent = e.message;
  } finally {
    result.hidden = false;
    button.disabled = false;
    button.textContent = "EINREISE PRÜFEN →";
  }
};
load();
