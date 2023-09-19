const frequencys = {C4:261.6, D4: 293.7, E4:329.6, F4: 349.2, G4:392.0,A4:440.0, B4: 493.9};
const audioContext = new AudioContext();
AudioBeep = {
    StartAudio: function (frequency) {
        const oscillator = audioContext.createOscillator();
        const gainNode = audioContext.createGain();
        const duration = 5;
        oscillator.type = 'sine';
        oscillator.frequency.setValueAtTime(frequency, audioContext.currentTime);
        oscillator.connect(gainNode);
        gainNode.connect(audioContext.destination);
        gainNode.gain.setValueAtTime(0, audioContext.currentTime);
        gainNode.gain.linearRampToValueAtTime(1, audioContext.currentTime + 0.01);
        oscillator.start(audioContext.currentTime);
        gainNode.gain.exponentialRampToValueAtTime(0.001, audioContext.currentTime + duration);
        oscillator.stop(audioContext.currentTime + duration);
    }
}
$(function () {
    window.setTimeout(function () {
        AudioBeep.StartAudio(frequencys.C4);
    }, 3000);
    window.setTimeout(function () {
        AudioBeep.StartAudio(frequencys.D4);
    }, 3000);
});