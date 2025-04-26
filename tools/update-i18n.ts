import * as fs from 'node:fs';
import * as path from 'node:path';

// @ts-ignore
const i18nDir: string = path.join(import.meta.dirname, '../Umbra/i18n');
const sourceFiles: SourceFile[] = collectSourceFiles();
const i18nFiles: I18NDict = loadI18nFiles();

const whitelist = [
    'CVAR',
    'Color',
    'Widget.Volume.Channel.',
    'Widget.Volume.Option.',
    'WeatherForecast.In',
];

let totalKeys = 0;

const unusedKeys: string[] = [];

for (const key of Object.keys(i18nFiles['en'])) {
    if (whitelist.find(w => key.startsWith(w))) continue;
    totalKeys++;

    if (!keyIsUsed(key)) {
        unusedKeys.push(key);
        delete i18nFiles['en'][key];
    }
}

console.log(`Found ${unusedKeys.length} unused keys.`);

// fs.writeFileSync(path.resolve(i18nDir, 'en.json'), JSON.stringify(i18nFiles['en'], null, 4), 'utf-8');



// for (const lang of Object.keys(i18nFiles)) {
//     if (lang === 'en') continue;
//     const en = i18nFiles['en'];
//     let deletedKeys = 0;
//    
//     for (const key of Object.keys(i18nFiles[lang])) {
//         if (typeof en[key] === 'undefined') {
//             delete i18nFiles[lang][key];
//             deletedKeys++;
//         }
//     }
//    
//     console.log(`Deleted ${deletedKeys} keys from ${lang}`);
//     if (deletedKeys > 0) {
//         fs.writeFileSync(path.resolve(i18nDir, lang + '.json'), JSON.stringify(en, null, 4), 'utf-8');
//     }
// }
//
// console.log(`Unused keys: ${unusedKeys.length} out of ${totalKeys}`);

// ------------------------------------------------------------------------- //

function keyIsUsed(key: string): boolean {
    for (const file of sourceFiles) {
        if (file.source.includes(key)) {
            return true;
        }
    }

    return false;
}

function loadI18nFiles() {
    const result: I18NDict = {};

    // @ts-ignore
    fs.readdirSync(i18nDir).forEach(file => {
        if (!file.endsWith('.json')) return;
        const lang = file.replace('.json', '');
        const data = JSON.parse(fs.readFileSync(path.resolve(i18nDir, file)).toString('utf-8').replace(/^\uFEFF/, ''));

        result[lang] = data;
    });

    return result;
}

function collectSourceFiles() {
    // @ts-ignore
    return scanSourceFiles(path.resolve(import.meta.dirname, '..', 'Umbra'));
}

function scanSourceFiles(dir: string, output: SourceFile[] = []) {
    fs.readdirSync(dir).map(f => path.resolve(dir, f)).forEach(file => {
        if (fs.statSync(file).isDirectory()) {
            scanSourceFiles(file, output);
            return;
        }

        if (file.endsWith('.cs')) {
            output.push({
                name: path.resolve(file),
                source: fs.readFileSync(file, 'utf-8'),
            })
        }
    });

    return output;
}

type SourceFile = {
    name: string;
    source: string;
}

type I18NDict = {
    [lang: string]: {
        [key: string]: string;
    }
}